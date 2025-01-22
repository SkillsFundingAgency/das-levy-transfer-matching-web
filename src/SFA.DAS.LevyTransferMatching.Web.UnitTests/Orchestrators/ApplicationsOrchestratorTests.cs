using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Orchestrators;

[TestFixture]
public class ApplicationsOrchestratorTests
{
    private readonly Fixture _fixture = new();

    private Mock<IApplicationsService> _mockApplicationsService;
    private Mock<IEncodingService> _mockEncodingService;
    private Mock<IUserService> _mockUserService;
    private ApplicationsOrchestrator _applicationsOrchestrator;

    [SetUp]
    public void Arrange()
    {
        _mockApplicationsService = new Mock<IApplicationsService>();
        _mockUserService = new Mock<IUserService>();
        _mockEncodingService = new Mock<IEncodingService>();
        var featureToggles = _fixture.Create<Infrastructure.Configuration.FeatureToggles>();

        _mockUserService.Setup(x => x.IsOwnerOrTransactor(It.IsAny<string>())).Returns(true);

        _applicationsOrchestrator = new ApplicationsOrchestrator(_mockApplicationsService.Object,
            _mockEncodingService.Object, featureToggles, _mockUserService.Object);
    }

    [Test]
    public async Task GetApplications_ShouldReturnViewModel_WithCorrectData()
    {
        // Arrange
        var request = _fixture.Create<GetApplicationsRequest>();
        var cancellationToken = CancellationToken.None;
        var pagedResponse = _fixture.Build<GetApplicationsResponse>()
            .With(x => x.Items, _fixture.CreateMany<GetApplicationsResponse.Application>(5).ToList())
            .Create();

        _mockApplicationsService
            .Setup(x => x.GetApplications(request.AccountId, request.Page.Value, It.IsAny<int>(), cancellationToken))
            .ReturnsAsync(pagedResponse);

        _mockEncodingService
            .Setup(x => x.Encode(It.IsAny<int>(), It.IsAny<EncodingType>()))
            .Returns<int, EncodingType>((id, type) => $"Encoded{type}_{id}");

        // Act
        var result = await _applicationsOrchestrator.GetApplications(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Applications.Should().HaveCount(pagedResponse.Items.Count());
        result.EncodedAccountId.Should().Be(request.EncodedAccountId);
        result.RenderViewApplicationDetailsHyperlink.Should().BeTrue();

        result.Paging.Should().NotBeNull();
        result.Paging.Page.Should().Be(pagedResponse.Page);
        result.Paging.PageSize.Should().Be(pagedResponse.PageSize);
        result.Paging.TotalPages.Should().Be(pagedResponse.TotalPages);
        result.Paging.TotalItems.Should().Be(pagedResponse.TotalItems);
        result.Paging.ShowPageLinks.Should().Be(pagedResponse.Page != 1 || pagedResponse.TotalItems > pagedResponse.PageSize);
    }

    [Test]
    public async Task GetApplications_ShouldReturnViewModel_WithCorrectPagingLinks()
    {
        // Arrange
        var request = _fixture.Create<GetApplicationsRequest>();
        var cancellationToken = CancellationToken.None;
        var pagedResponse = _fixture.Build<GetApplicationsResponse>()
            .With(x => x.Page, 3)
            .With(x => x.PageSize, 10)
            .With(x => x.TotalItems, 50)
            .With(x => x.TotalPages, 5)
            .With(x => x.Items, _fixture.CreateMany<GetApplicationsResponse.Application>(10).ToList())
            .Create();

        _mockApplicationsService
            .Setup(x => x.GetApplications(request.AccountId, request.Page.Value, GetApplicationsRequest.PageSize, cancellationToken))
            .ReturnsAsync(pagedResponse);

        _mockEncodingService
            .Setup(x => x.Encode(It.IsAny<int>(), It.IsAny<EncodingType>()))
            .Returns<int, EncodingType>((id, type) => $"Encoded{type}_{id}");


        // Act
        var result = await _applicationsOrchestrator.GetApplications(request, cancellationToken);

        // Assert
        var pageLinks = result.Paging.PageLinks.ToList();
        pageLinks.Should().HaveCount(7);

        // Verify Previous link
        pageLinks[0].Label.Should().Be("Previous");
        pageLinks[0].AriaLabel.Should().Be("Previous page");

        // Verify numbered links
        pageLinks[1].Label.Should().Be("1");
        pageLinks[1].AriaLabel.Should().Be("Page 1");
        pageLinks[1].IsCurrent.Should().BeNull();

        pageLinks[2].Label.Should().Be("2");
        pageLinks[2].AriaLabel.Should().Be("Page 2");
        pageLinks[2].IsCurrent.Should().BeNull();

        pageLinks[3].Label.Should().Be("3");
        pageLinks[3].AriaLabel.Should().Be("Page 3");
        pageLinks[3].IsCurrent.Should().BeTrue();

        pageLinks[4].Label.Should().Be("4");
        pageLinks[4].AriaLabel.Should().Be("Page 4");
        pageLinks[4].IsCurrent.Should().BeNull();

        pageLinks[5].Label.Should().Be("5");
        pageLinks[5].AriaLabel.Should().Be("Page 5");
        pageLinks[5].IsCurrent.Should().BeNull();

        // Verify Next link
        pageLinks[6].Label.Should().Be("Next");
        pageLinks[6].AriaLabel.Should().Be("Next page");
    }

    [Test]
    public async Task GetApplicationViewModel_ApplicationExists_ReturnsViewModel()
    {
        // Arrange
        var request = _fixture.Create<ApplicationRequest>();
        var response = _fixture.Create<GetApplicationResponse>();

        var encodedPledgeId = _fixture.Create<string>();

        _mockApplicationsService
            .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId),
                It.Is<int>(y => y == request.ApplicationId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        _mockEncodingService
            .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId),
                It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
            .Returns(encodedPledgeId);

        // Act
        var viewModel = await _applicationsOrchestrator.GetApplication(request);

        Assert.Multiple(() =>
        {
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.EncodedOpportunityId, Is.EqualTo(encodedPledgeId));
        });
    }


    [TestCase(ApplicationStatus.Pending, true)]
    [TestCase(ApplicationStatus.Approved, false)]
    public async Task GetApplicationViewModel_CanWithdraw_Is_True_When_Application_Is_Pending(
        ApplicationStatus status, bool expectCanWithdraw)
    {
        // Arrange
        var request = _fixture.Create<ApplicationRequest>();
        var response = _fixture.Create<GetApplicationResponse>();
        response.Status = status;

        var encodedPledgeId = _fixture.Create<string>();

        _mockApplicationsService
            .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId),
                It.Is<int>(y => y == request.ApplicationId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        _mockEncodingService
            .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId),
                It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
            .Returns(encodedPledgeId);

        // Act
        var viewModel = await _applicationsOrchestrator.GetApplication(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.CanWithdraw, Is.EqualTo(expectCanWithdraw));
        });
    }

    [Test]
    public async Task GetApplicationViewModel_ApplicationDoesntExist_ReturnsNull()
    {
        // Arrange
        var request = _fixture.Create<ApplicationRequest>();

        _mockApplicationsService
            .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId),
                It.Is<int>(y => y == request.ApplicationId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetApplicationResponse)null);

        // Act
        var viewModel = await _applicationsOrchestrator.GetApplication(request);

        // Assert
        Assert.That(viewModel, Is.Null);
    }

    [Test]
    public async Task SetApplicationAcceptance_OuterApiCalled_RequestMappedCorrectly()
    {
        // Arrange
        var request = _fixture.Create<ApplicationPostRequest>();

        var expectedUserId = _fixture.Create<string>();
        var expectedUserDisplayName = _fixture.Create<string>();

        var expectedAcceptance = request.SelectedAction == ApplicationViewModel.ApprovalAction.Accept
            ? SetApplicationAcceptanceRequest.ApplicationAcceptance.Accept
            : SetApplicationAcceptanceRequest.ApplicationAcceptance.Decline;

        SetApplicationAcceptanceRequest actualRequest = null;
        Action<SetApplicationAcceptanceRequest, CancellationToken> setApplicationAcceptanceCallback =
            (x, y) => { actualRequest = x; };

        _mockUserService
            .Setup(x => x.GetUserId())
            .Returns(expectedUserId);
        _mockUserService
            .Setup(x => x.GetUserDisplayName())
            .Returns(expectedUserDisplayName);

        _mockApplicationsService
            .Setup(x => x.SetApplicationAcceptance(It.IsAny<SetApplicationAcceptanceRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback(setApplicationAcceptanceCallback);

        // Act
        await _applicationsOrchestrator.SetApplicationAcceptance(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actualRequest, Is.Not.Null);
            Assert.That(actualRequest.UserId, Is.EqualTo(expectedUserId));
            Assert.That(actualRequest.UserDisplayName, Is.EqualTo(expectedUserDisplayName));
            Assert.That(actualRequest.Acceptance, Is.EqualTo(expectedAcceptance));
        });
    }

    [Test]
    public async Task GetAcceptedViewModel_ApplicationExists_ReturnsViewModel()
    {
        // Arrange
        var request = _fixture.Create<AcceptedRequest>();
        var response = _fixture.Create<GetAcceptedResponse>();
        var encodedPledgeId = _fixture.Create<string>();

        _mockApplicationsService
            .Setup(x => x.GetAccepted(It.Is<long>(y => y == request.AccountId),
                It.Is<int>(y => y == request.ApplicationId)))
            .ReturnsAsync(response);

        _mockEncodingService
            .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId),
                It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
            .Returns(encodedPledgeId);

        // Act
        var viewModel = await _applicationsOrchestrator.GetAcceptedViewModel(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.EmployerNameAndReference,
                Is.EqualTo($"{response.EmployerAccountName} ({encodedPledgeId})"));
        });
    }

    [Test]
    public async Task GetAcceptedViewModel_ApplicationDoesntExist_ReturnsNull()
    {
        // Arrange
        var request = _fixture.Create<AcceptedRequest>();

        _mockApplicationsService
            .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId),
                It.Is<int>(y => y == request.ApplicationId), CancellationToken.None))
            .ReturnsAsync((GetApplicationResponse)null);

        // Act
        var viewModel = await _applicationsOrchestrator.GetAcceptedViewModel(request);

        // Assert
        Assert.That(viewModel, Is.Null);
    }

    [Test]
    public async Task GetDeclinedViewModel_ApplicationExists_ReturnsViewModel()
    {
        // Arrange
        var request = _fixture.Create<DeclinedRequest>();
        var response = _fixture.Create<GetDeclinedResponse>();
        var encodedPledgeId = _fixture.Create<string>();

        _mockApplicationsService
            .Setup(x => x.GetDeclined(It.Is<long>(y => y == request.AccountId),
                It.Is<int>(y => y == request.ApplicationId)))
            .ReturnsAsync(response);

        _mockEncodingService
            .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId),
                It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
            .Returns(encodedPledgeId);

        // Act
        var viewModel = await _applicationsOrchestrator.GetDeclinedViewModel(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.EmployerNameAndReference,
                Is.EqualTo($"{response.EmployerAccountName} ({encodedPledgeId})"));
        });
    }

    [Test]
    public async Task GetDeclinedViewModel_ApplicationDoesntExist_ReturnsNull()
    {
        // Arrange
        var request = _fixture.Create<DeclinedRequest>();

        _mockApplicationsService
            .Setup(x => x.GetDeclined(It.Is<long>(y => y == request.AccountId),
                It.Is<int>(y => y == request.ApplicationId)))
            .ReturnsAsync((GetDeclinedResponse)null);

        // Act
        var viewModel = await _applicationsOrchestrator.GetDeclinedViewModel(request);

        // Assert
        Assert.That(viewModel, Is.Null);
    }

    [Test]
    public async Task
        GetApplicationViewModel_IsOwnerAndTransactorAndStatusEqualsApproved_ReturnsViewModelWithCanAcceptFunding()
    {
        // Arrange
        var request = _fixture.Create<ApplicationRequest>();
        var response = _fixture.Create<GetApplicationResponse>();
        var encodedPledgeId = _fixture.Create<string>();

        _mockApplicationsService
            .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId),
                It.Is<int>(y => y == request.ApplicationId), CancellationToken.None))
            .ReturnsAsync(response);

        _mockEncodingService
            .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId),
                It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
            .Returns(encodedPledgeId);

        response.Status = ApplicationStatus.Approved;
        _mockUserService.Setup(o => o.IsOwnerOrTransactor(It.IsAny<string>())).Returns(true);

        // Act
        var viewModel = await _applicationsOrchestrator.GetApplication(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.CanAcceptFunding, Is.EqualTo(true));
        });
    }

    [Test]
    public async Task
        GetApplicationViewModel_IsOwnerAndTransactorAndStatusEqualsAccepted_ReturnsViewModelWithCanUseTransferFunds()
    {
        // Arrange
        var request = _fixture.Create<ApplicationRequest>();
        var response = _fixture.Create<GetApplicationResponse>();
        var encodedPledgeId = _fixture.Create<string>();

        _mockApplicationsService
            .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId),
                It.Is<int>(y => y == request.ApplicationId), CancellationToken.None))
            .ReturnsAsync(response);

        _mockEncodingService
            .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId),
                It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
            .Returns(encodedPledgeId);

        response.Status = ApplicationStatus.Accepted;
        _mockUserService.Setup(o => o.IsOwnerOrTransactor(It.IsAny<string>())).Returns(true);

        // Act
        var viewModel = await _applicationsOrchestrator.GetApplication(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.CanUseTransferFunds, Is.EqualTo(true));
        });
    }

    [Test]
    public async Task
        GetApplicationViewModel_IsOwnerAndTransactorAndStatusEqualsAcceptedAndIsWithdrawable_ReturnsViewModelWithRenderWithdrawButton()
    {
        // Arrange
        var request = _fixture.Create<ApplicationRequest>();
        var response = _fixture.Create<GetApplicationResponse>();
        var encodedPledgeId = _fixture.Create<string>();

        _mockApplicationsService
            .Setup(x => x.GetApplication(It.Is<long>(y => y == request.AccountId),
                It.Is<int>(y => y == request.ApplicationId), CancellationToken.None))
            .ReturnsAsync(response);

        _mockEncodingService
            .Setup(x => x.Encode(It.Is<long>(y => y == response.OpportunityId),
                It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
            .Returns(encodedPledgeId);

        response.Status = ApplicationStatus.Accepted;
        response.IsWithdrawableAfterAcceptance = true;
        _mockUserService.Setup(o => o.IsOwnerOrTransactor(It.IsAny<string>())).Returns(true);

        // Act
        var viewModel = await _applicationsOrchestrator.GetApplication(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.RenderWithdrawAfterAcceptanceButton, Is.EqualTo(true));
        });
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
            .Setup(x => x.GetWithdrawalConfirmation(It.Is<long>(y => y == request.AccountId),
                It.Is<int>(y => y == request.ApplicationId)))
            .ReturnsAsync(response);

        _mockEncodingService
            .Setup(x => x.Encode(It.Is<long>(y => y == response.PledgeId),
                It.Is<EncodingType>(y => y == EncodingType.PledgeId)))
            .Returns(encodedPledgeId);

        _mockEncodingService
            .Setup(x => x.Encode(It.Is<long>(y => y == request.AccountId),
                It.Is<EncodingType>(y => y == EncodingType.AccountId)))
            .Returns(encodedAccountId);

        _mockEncodingService
            .Setup(x => x.Encode(It.Is<long>(y => y == request.ApplicationId),
                It.Is<EncodingType>(y => y == EncodingType.PledgeApplicationId)))
            .Returns(encodedApplicationId);

        // Act
        var viewModel = await _applicationsOrchestrator.GetWithdrawalConfirmationViewModel(request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.PledgeEmployerName, Is.EqualTo(response.PledgeEmployerName));
            Assert.That(viewModel.EncodedPledgeId, Is.EqualTo(encodedPledgeId));
            Assert.That(viewModel.EncodedAccountId, Is.EqualTo(encodedAccountId));
            Assert.That(viewModel.EncodedApplicationId, Is.EqualTo(encodedApplicationId));
        });
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
            It.Is<WithdrawApplicationAfterAcceptanceRequest>(y =>
                y.UserId == expectedUserId && y.UserDisplayName == expectedUserDisplayName),
            request.AccountId,
            request.ApplicationId), Times.Once);
    }
}