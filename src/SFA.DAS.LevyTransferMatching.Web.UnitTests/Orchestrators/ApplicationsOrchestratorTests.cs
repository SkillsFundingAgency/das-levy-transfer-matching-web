using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
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
        private readonly Fixture _fixture = new Fixture();

        private Mock<IApplicationsService> _mockApplicationsService;
        private Mock<IDateTimeService> _mockDateTimeService;
        private Mock<IEncodingService> _mockEncodingService;
        private Mock<IUserService> _mockUserService;
        private ApplicationsOrchestrator _applicationsOrchestrator;

        [SetUp]
        public void Arrange()
        {
            _mockApplicationsService = new Mock<IApplicationsService>();
            _mockDateTimeService = new Mock<IDateTimeService>();

            _mockDateTimeService
                .Setup(x => x.UtcNow)
                .Returns(_fixture.Create<DateTime>());
            _mockUserService = new Mock<IUserService>();
            _mockEncodingService = new Mock<IEncodingService>();
            var featureToggles = _fixture.Create<Infrastructure.Configuration.FeatureToggles>();

            _mockUserService.Setup(x => x.IsOwnerOrTransactor(It.IsAny<string>())).Returns(true);

            _applicationsOrchestrator = new ApplicationsOrchestrator(_mockApplicationsService.Object, _mockDateTimeService.Object, _mockEncodingService.Object, featureToggles, _mockUserService.Object);
        }

        [Test]
        public async Task GetApplicationViewModel_ApplicationExists_ReturnsViewModel()
        {
            // Arrange
            var request = _fixture.Create<ApplicationRequest>();
            var response = _fixture.Create <GetApplicationResponse>();

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


        [TestCase(ApplicationStatus.Pending, true)]
        [TestCase(ApplicationStatus.Approved, false)]
        public async Task GetApplicationViewModel_CanWithdraw_Is_True_When_Application_Is_Pending(ApplicationStatus status, bool expectCanWithdraw)
        {
            // Arrange
            var request = _fixture.Create<ApplicationRequest>();
            var response = _fixture.Create<GetApplicationResponse>();
            response.Status = status;

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
            Assert.AreEqual(expectCanWithdraw, viewModel.CanWithdraw);
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

        [Test]
        public async Task GetAcceptedViewModel_ApplicationExists_ReturnsViewModel()
        {
            // Arrange
            var request = _fixture.Create<AcceptedRequest>();
            var response = _fixture.Create<GetAcceptedResponse>();
            var encodedPledgeId = _fixture.Create<string>();

            _mockApplicationsService
                .Setup(x => x.GetAccepted(It.Is<long>(y => y == request.AccountId), It.Is<int>(y => y == request.ApplicationId)))
                .ReturnsAsync(response);

            _mockEncodingService
                .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId), It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
                .Returns(encodedPledgeId);

            // Act
            var viewModel = await _applicationsOrchestrator.GetAcceptedViewModel(request);

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual($"{response.EmployerAccountName} ({encodedPledgeId})", viewModel.EmployerNameAndReference);
        }

        [Test]
        public async Task GetAcceptedViewModel_ApplicationDoesntExist_ReturnsNull()
        {
            // Arrange
            var request = _fixture.Create<AcceptedRequest>();

            _mockApplicationsService
                .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId), It.Is<int>(y => y == request.ApplicationId), CancellationToken.None))
                .ReturnsAsync((GetApplicationResponse)null);

            // Act
            var viewModel = await _applicationsOrchestrator.GetAcceptedViewModel(request);

            // Assert
            Assert.IsNull(viewModel);
        }

        [Test]
        public async Task GetDeclinedViewModel_ApplicationExists_ReturnsViewModel()
        {
            // Arrange
            var request = _fixture.Create<DeclinedRequest>();
            var response = _fixture.Create<GetDeclinedResponse>();
            var encodedPledgeId = _fixture.Create<string>();

            _mockApplicationsService
                .Setup(x => x.GetDeclined(It.Is<long>(y => y == request.AccountId), It.Is<int>(y => y == request.ApplicationId)))
                .ReturnsAsync(response);

            _mockEncodingService
                .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId), It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
                .Returns(encodedPledgeId);

            // Act
            var viewModel = await _applicationsOrchestrator.GetDeclinedViewModel(request);

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual($"{response.EmployerAccountName} ({encodedPledgeId})", viewModel.EmployerNameAndReference);
        }

        [Test]
        public async Task GetDeclinedViewModel_ApplicationDoesntExist_ReturnsNull()
        {
            // Arrange
            var request = _fixture.Create<DeclinedRequest>();

            _mockApplicationsService
                .Setup(x => x.GetDeclined(It.Is<long>(y => y == request.AccountId), It.Is<int>(y => y == request.ApplicationId)))
                .ReturnsAsync((GetDeclinedResponse)null);

            // Act
            var viewModel = await _applicationsOrchestrator.GetDeclinedViewModel(request);

            // Assert
            Assert.IsNull(viewModel);
        }

        [Test]
        public async Task GetApplicationViewModel_IsOwnerAndTransactorAndStatusEqualsApproved_ReturnsViewModelWithCanAcceptFunding()
        {
            // Arrange
            var request = _fixture.Create<ApplicationRequest>();
            var response = _fixture.Create<GetApplicationResponse>();
            var encodedPledgeId = _fixture.Create<string>();

            _mockApplicationsService
                .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId), It.Is<int>(y => y == request.ApplicationId), CancellationToken.None))
                .ReturnsAsync(response);

            _mockEncodingService
                .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId), It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
                .Returns(encodedPledgeId);
            
            response.Status = ApplicationStatus.Approved;
            _mockUserService.Setup(o => o.IsOwnerOrTransactor(It.IsAny<string>())).Returns(true);

            // Act
            var viewModel = await _applicationsOrchestrator.GetApplication(request);

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(true, viewModel.CanAcceptFunding);
        }

        [Test]
        public async Task GetApplicationViewModel_IsOwnerAndTransactorAndStatusEqualsAccepted_ReturnsViewModelWithCanUseTransferFunds()
        {
            // Arrange
            var request = _fixture.Create<ApplicationRequest>();
            var response = _fixture.Create<GetApplicationResponse>();
            var encodedPledgeId = _fixture.Create<string>();

            _mockApplicationsService
                .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId), It.Is<int>(y => y == request.ApplicationId), CancellationToken.None))
                .ReturnsAsync(response);

            _mockEncodingService
                .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId), It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
                .Returns(encodedPledgeId);

            response.Status = ApplicationStatus.Accepted;
            _mockUserService.Setup(o => o.IsOwnerOrTransactor(It.IsAny<string>())).Returns(true);

            // Act
            var viewModel = await _applicationsOrchestrator.GetApplication(request);

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(true, viewModel.CanUseTransferFunds);
        }

        [Test]
        public async Task GetApplicationViewModel_IsOwnerAndTransactorAndStatusEqualsAcceptedAndIsWithdrawable_ReturnsViewModelWithRenderWithdrawButton()
        {
            // Arrange
            var request = _fixture.Create<ApplicationRequest>();
            var response = _fixture.Create<GetApplicationResponse>();
            var encodedPledgeId = _fixture.Create<string>();

            _mockApplicationsService
                .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId), It.Is<int>(y => y == request.ApplicationId), CancellationToken.None))
                .ReturnsAsync(response);

            _mockEncodingService
                .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId), It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
                .Returns(encodedPledgeId);

            response.Status = ApplicationStatus.Accepted;
            response.IsWithdrawableAfterAcceptance = true;
            _mockUserService.Setup(o => o.IsOwnerOrTransactor(It.IsAny<string>())).Returns(true);

            // Act
            var viewModel = await _applicationsOrchestrator.GetApplication(request);

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(true, viewModel.RenderWithdrawAfterAcceptanceButton);
        }

        [Test]
        public async Task GetWithdrawalConfirmationViewModel_ReturnsViewModel()
        {
            // Arrange
            var request = _fixture.Create<WithdrawalConfirmationRequest>();
            var response = _fixture.Create<GetWithdrawalConfirmationResponse>();
            var encodedPledgeId = _fixture.Create<string>();
            var encodedAccountId = _fixture.Create<string>();
            var encodedApplicationId = _fixture.Create<string>();

            _mockApplicationsService
                .Setup(x => x.GetWithdrawalConfirmation(It.Is<long>(y => y == request.AccountId), It.Is<int>(y => y == request.ApplicationId)))
                .ReturnsAsync(response);

            _mockEncodingService
                .Setup(x => x.Encode(It.Is<long>(y => y == response.PledgeId), It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
                .Returns(encodedPledgeId);

            _mockEncodingService
                .Setup(x => x.Encode(It.Is<long>(y => y == request.AccountId), It.Is<EncodingType>(y => y == EncodingType.AccountId)))
                .Returns(encodedAccountId);

            _mockEncodingService
                .Setup(x => x.Encode(It.Is<long>(y => y == request.ApplicationId), It.Is<EncodingType>(y => y == EncodingType.PledgeApplicationId)))
                .Returns(encodedApplicationId);

            // Act
            var viewModel = await _applicationsOrchestrator.GetWithdrawalConfirmationViewModel(request);

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(response.PledgeEmployerName, viewModel.PledgeEmployerName);
            Assert.AreEqual(encodedPledgeId, viewModel.EncodedPledgeId);
            Assert.AreEqual(encodedAccountId, viewModel.EncodedAccountId);
            Assert.AreEqual(encodedApplicationId, viewModel.EncodedApplicationId);
        }

        [Test]
        public async Task WithdrawConfirmationAfterAcceptance_CallsService()
        {
            // Arrange
            var request = _fixture.Create<ConfirmWithdrawalPostRequest>();
            var expectedUserId = _fixture.Create<string>();
            var expectedUserDisplayName = _fixture.Create<string>();

            _mockUserService
                .Setup(x => x.GetUserId())
                .Returns(expectedUserId);
            _mockUserService
                .Setup(x => x.GetUserDisplayName())
                .Returns(expectedUserDisplayName);

            // Act
            await _applicationsOrchestrator.WithdrawApplicationAfterAcceptance(request);

            // Assert
            _mockApplicationsService.Verify(x => x.WithdrawApplicationAfterAcceptance(
                                                    It.Is<WithdrawApplicationAfterAcceptanceRequest>(y => y.UserId == expectedUserId && y.UserDisplayName == expectedUserDisplayName),
                                                    request.AccountId,
                                                    request.ApplicationId), Times.Once);
        }
    }
}