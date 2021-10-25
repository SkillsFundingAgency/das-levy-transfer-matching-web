using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class ApplicationsOrchestratorTests
    {
        private Fixture _fixture;

        private Mock<IApplicationsService> _mockApplicationsService;
        private Mock<IDateTimeService> _mockDateTimeService;
        private Mock<IEncodingService> _mockEncodingService;
        private Mock<IUserService> _mockUserService;
        private ApplicationsOrchestrator _applicationsOrchestrator;

        [SetUp]
        public void Arrange()
        {
            _fixture = new Fixture();

            _mockApplicationsService = new Mock<IApplicationsService>();
            _mockDateTimeService = new Mock<IDateTimeService>();

            _mockDateTimeService
                .Setup(x => x.UtcNow)
                .Returns(_fixture.Create<DateTime>());
            _mockUserService = new Mock<IUserService>();
            _mockEncodingService = new Mock<IEncodingService>();
            var featureToggles = _fixture.Create<Infrastructure.Configuration.FeatureToggles>();

            _applicationsOrchestrator = new ApplicationsOrchestrator(_mockApplicationsService.Object, _mockDateTimeService.Object, _mockEncodingService.Object, featureToggles, _mockUserService.Object);
        }

        [Test]
        public async Task GetApplicationViewModel_ApplicationExists_ReturnsViewModel()
        {
            // Arrange
            var request = _fixture.Create<ApplicationRequest>();
            var response = _fixture.Create <GetApplicationResponse>();

            // Because -
            // Random dates don't play well with ApprenticeshipFundingDtoExtensions.GetEffectiveFundingLine
            response.Standard.ApprenticeshipFunding.First().EffectiveFrom = DateTime.Now.AddMonths(-3);
            response.Standard.ApprenticeshipFunding.First().EffectiveTo = DateTime.Now.AddYears(2);
            response.StartBy = DateTime.Now.AddMonths(2);

            var encodedPledgeId = _fixture.Create<string>();

            _mockApplicationsService
                .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId), It.Is<int>(y => y == request.ApplicationId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _mockEncodingService
                .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId), It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
                .Returns(encodedPledgeId);

            // Act
            var viewModel = await _applicationsOrchestrator.GetApplication(request);

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(encodedPledgeId, viewModel.EncodedOpportunityId);
        }

        [Test]
        public async Task GetApplicationViewModel_ApplicationDoesntExist_ReturnsNull()
        {
            // Arrange
            var request = _fixture.Create<ApplicationRequest>();

            _mockApplicationsService
                .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId), It.Is<int>(y => y == request.ApplicationId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetApplicationResponse)null);

            // Act
            var viewModel = await _applicationsOrchestrator.GetApplication(request);

            // Assert
            Assert.IsNull(viewModel);
        }

        [Test]
        public async Task SetApplicationAcceptance_OuterApiCalled_RequestMappedCorrectly()
        {
            // Arrange
            var request = _fixture.Create<ApplicationPostRequest>();

            var expectedUserId = _fixture.Create<string>();
            var expectedUserDisplayName = _fixture.Create<string>();

            var expectedAcceptance = request.SelectedAction == ApplicationViewModel.ApprovalAction.Accept ?
                SetApplicationAcceptanceRequest.ApplicationAcceptance.Accept :
                SetApplicationAcceptanceRequest.ApplicationAcceptance.Decline;

            SetApplicationAcceptanceRequest actualRequest = null;
            Action<SetApplicationAcceptanceRequest, CancellationToken> setApplicationAcceptanceCallback =
                (x, y) =>
                {
                    actualRequest = x;
                };

            _mockUserService
                .Setup(x => x.GetUserId())
                .Returns(expectedUserId);
            _mockUserService
                .Setup(x => x.GetUserDisplayName())
                .Returns(expectedUserDisplayName);

            _mockApplicationsService
                .Setup(x => x.SetApplicationAcceptance(It.IsAny<SetApplicationAcceptanceRequest>(), It.IsAny<CancellationToken>()))
                .Callback(setApplicationAcceptanceCallback);

            // Act
            await _applicationsOrchestrator.SetApplicationAcceptance(request);

            // Assert
            Assert.NotNull(actualRequest);
            Assert.AreEqual(expectedUserId, actualRequest.UserId);
            Assert.AreEqual(expectedUserDisplayName, actualRequest.UserDisplayName);
            Assert.AreEqual(expectedAcceptance, actualRequest.Acceptance);
        }
    }
}