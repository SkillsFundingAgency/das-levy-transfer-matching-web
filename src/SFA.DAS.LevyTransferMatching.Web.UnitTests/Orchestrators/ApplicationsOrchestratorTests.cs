using System;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
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

            _mockEncodingService = new Mock<IEncodingService>();
            var featureToggles = _fixture.Create<Infrastructure.Configuration.FeatureToggles>();

            _applicationsOrchestrator = new ApplicationsOrchestrator(_mockApplicationsService.Object, _mockDateTimeService.Object, _mockEncodingService.Object, featureToggles);
        }

        [Test]
        public async Task GetApplicationViewModel_ApplicationExists_ReturnsViewModel()
        {
            // Arrange
            var request = _fixture.Create<ApplicationRequest>();
            var response = _fixture.Create<GetApplicationResponse>();
            var encodedPledgeId = _fixture.Create<string>();

            _mockApplicationsService
                .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId), It.Is<int>(y => y == request.ApplicationId)))
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
                .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId), It.Is<int>(y => y == request.ApplicationId)))
                .ReturnsAsync((GetApplicationResponse)null);

            // Act
            var viewModel = await _applicationsOrchestrator.GetApplication(request);

            // Assert
            Assert.IsNull(viewModel);
        }
    }
}