using SFA.DAS.LevyTransferMatching.Web.Controllers;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers;

public class ApplicationsControllerTests
{
    private Fixture _fixture;
    private Mock<IApplicationsOrchestrator> _orchestrator;
    private ApplicationsController _controller;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _orchestrator = new Mock<IApplicationsOrchestrator>();
        _controller = new ApplicationsController(_orchestrator.Object);
    }

    [TearDown]
    public void TearDown() => _controller?.Dispose();

    [Test]
    public async Task GET_GetApplications_ReturnsViewAndModel()
    {
        _orchestrator.Setup(o =>
                o.GetApplications(It.IsAny<GetApplicationsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetApplicationsViewModel
            {
                EncodedAccountId = "ID"
            });

        var result = await _controller.Applications(new GetApplicationsRequest()) as ViewResult;

        var actual = result.Model as GetApplicationsViewModel;
        Assert.That(actual.EncodedAccountId, Is.EqualTo("ID"));
    }

    [Test]
    public async Task GET_Application_ApplicationExists_ReturnsViewAndModel()
    {
        // Arrange
        var request = _fixture.Create<ApplicationRequest>();
        _orchestrator
            .Setup(x => x.GetApplication(It.Is<ApplicationRequest>(y => y == request)))
            .ReturnsAsync(new ApplicationViewModel());

        // Act
        var actionResult = await _controller.Application(request);
        var viewResult = actionResult as ViewResult;
        var model = viewResult.Model;
        var applicationViewModel = model as ApplicationViewModel;

        // Assert
        Assert.That(actionResult, Is.Not.Null);
        Assert.That(viewResult, Is.Not.Null);
        Assert.That(model, Is.Not.Null);
        Assert.That(applicationViewModel, Is.Not.Null);
    }

    [Test]
    public async Task GET_Application_ApplicationDoesntExist_ReturnsNotFound()
    {
        // Arrange
        var request = _fixture.Create<ApplicationRequest>();
        _orchestrator
            .Setup(x => x.GetApplication(It.Is<ApplicationRequest>(y => y == request)))
            .ReturnsAsync((ApplicationViewModel)null);

        // Act
        var actionResult = await _controller.Application(request);
        var notFoundResult = actionResult as NotFoundResult;

        // Assert
        Assert.That(actionResult, Is.Not.Null);
        Assert.That(notFoundResult, Is.Not.Null);
    }

    [Test]
    public async Task GET_Accepted_ApplicationExists_ReturnsViewAndModel()
    {
        // Arrange
        var request = _fixture.Create<AcceptedRequest>();
        _orchestrator
            .Setup(x => x.GetAcceptedViewModel(It.Is<AcceptedRequest>(y => y == request)))
            .ReturnsAsync(new AcceptedViewModel());

        // Act
        var actionResult = await _controller.Accepted(request);
        var viewResult = actionResult as ViewResult;
        var model = viewResult.Model;
        var acceptedViewModel = model as AcceptedViewModel;

        // Assert
        Assert.That(actionResult, Is.Not.Null);
        Assert.That(viewResult, Is.Not.Null);
        Assert.That(model, Is.Not.Null);
        Assert.That(acceptedViewModel, Is.Not.Null);
    }

    [Test]
    public async Task GET_Accepted_ApplicationDoesntExist_ReturnsNotFound()
    {
        // Arrange
        var request = _fixture.Create<AcceptedRequest>();
        _orchestrator
            .Setup(x => x.GetAcceptedViewModel(It.Is<AcceptedRequest>(y => y == request)))
            .ReturnsAsync((AcceptedViewModel)null);

        // Act
        var actionResult = await _controller.Accepted(request);
        var notFoundResult = actionResult as NotFoundResult;

        // Assert
        Assert.That(actionResult, Is.Not.Null);
        Assert.That(notFoundResult, Is.Not.Null);
    }

    [Test]
    public async Task POST_Application_SelectedActionIsAccept_RedirectsToCorrectPath()
    {
        // Arrange
        var request = _fixture
            .Build<ApplicationPostRequest>()
            .With(x => x.SelectedAction, ApplicationViewModel.ApprovalAction.Accept)
            .Create();

        // Act
        var actionResult = await _controller.Application(request);
        var redirectResult = actionResult as RedirectResult;

        // Assert
        Assert.That(actionResult, Is.Not.Null);
        Assert.That(redirectResult, Is.Not.Null);
        Assert.That(
            redirectResult.Url,
            Is.EqualTo($"/accounts/{request.EncodedAccountId}/applications/{request.EncodedApplicationId}/accepted"));
    }

    [Test]
    public async Task POST_Application_SelectedActionIsDecline_RedirectsToCorrectPath()
    {
        // Arrange
        var request = _fixture
            .Build<ApplicationPostRequest>()
            .With(x => x.SelectedAction, ApplicationViewModel.ApprovalAction.Decline)
            .Create();

        // Act
        var actionResult = await _controller.Application(request);
        var redirectResult = actionResult as RedirectResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(redirectResult.Url,
                Is.EqualTo(
                    $"/accounts/{request.EncodedAccountId}/applications/{request.EncodedApplicationId}/declined"));
        });
    }

    [Test]
    public async Task GET_Declined_ApplicationExists_ReturnsViewAndModel()
    {
        // Arrange
        var request = _fixture.Create<DeclinedRequest>();
        _orchestrator
            .Setup(x => x.GetDeclinedViewModel(It.Is<DeclinedRequest>(y => y == request)))
            .ReturnsAsync(new DeclinedViewModel());

        // Act
        var actionResult = await _controller.Declined(request);
        var viewResult = actionResult as ViewResult;
        var model = viewResult.Model;
        var declinedViewModel = model as DeclinedViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(declinedViewModel, Is.Not.Null);
        });
    }

    [Test]
    public async Task GET_Declined_ApplicationDoesntExist_ReturnsNotFound()
    {
        // Arrange
        var request = _fixture.Create<DeclinedRequest>();
        _orchestrator
            .Setup(x => x.GetDeclinedViewModel(It.Is<DeclinedRequest>(y => y == request)))
            .ReturnsAsync((DeclinedViewModel)null);

        // Act
        var actionResult = await _controller.Declined(request);
        var notFoundResult = actionResult as NotFoundResult;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(notFoundResult, Is.Not.Null);
        });
    }

    [Test]
    public async Task GET_Withdrawn_ApplicationExists_ReturnsViewAndModel()
    {
        // Arrange
        var request = _fixture.Create<WithdrawnRequest>();
        _orchestrator
            .Setup(x => x.GetWithdrawnViewModel(It.Is<WithdrawnRequest>(y => y == request)))
            .ReturnsAsync(new WithdrawnViewModel());

        // Act
        var actionResult = await _controller.Withdrawn(request);
        var viewResult = actionResult as ViewResult;
        var model = viewResult.Model;
        var withdrawnViewModel = model as WithdrawnViewModel;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(withdrawnViewModel, Is.Not.Null);
        });
    }

    [Test]
    public async Task GET_Withdrawn_ApplicationDoesntExist_ReturnsNotFound()
    {
        // Arrange
        var request = _fixture.Create<WithdrawnRequest>();
        _orchestrator
            .Setup(x => x.GetWithdrawnViewModel(It.Is<WithdrawnRequest>(y => y == request)))
            .ReturnsAsync((WithdrawnViewModel)null);

        // Act
        var actionResult = await _controller.Withdrawn(request);
        var notFoundResult = actionResult as NotFoundResult;

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(notFoundResult, Is.Not.Null);
        });
    }

    [Test]
    public async Task GET_WithdrawalConfirmation_ReturnsViewAndModel()
    {
        // Arrange
        var request = _fixture.Create<WithdrawalConfirmationRequest>();
        _orchestrator
            .Setup(x => x.GetWithdrawalConfirmationViewModel(It.Is<WithdrawalConfirmationRequest>(y => y == request)))
            .ReturnsAsync(new WithdrawalConfirmationViewModel());

        // Act
        var actionResult = await _controller.WithdrawalConfirmation(request);
        var viewResult = actionResult as ViewResult;
        var model = viewResult.Model;
        var declinedViewModel = model as WithdrawalConfirmationViewModel;

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(model, Is.Not.Null);
            Assert.That(declinedViewModel, Is.Not.Null);
        });
    }

    [Test]
    public async Task POST_ConfirmWithdrawal_Returns_Expected_Redirect_When_Confirmed()
    {
        // Arrange
        var request = _fixture.Build<ConfirmWithdrawalPostRequest>()
            .With(x => x.HasConfirmed, true)
            .Create();

        // Act
        var actionResult = await _controller.ConfirmWithdrawal(request) as RedirectToActionResult;

        // Assert
        Assert.That(actionResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actionResult.ActionName, Is.EqualTo("Withdrawn"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
            Assert.That(actionResult.RouteValues["encodedApplicationId"], Is.EqualTo(request.EncodedApplicationId));
        });
    }

    [Test]
    public async Task POST_ConfirmWithdrawal_Returns_Expected_Redirect_When_Cancelled()
    {
        // Arrange
        var request = _fixture.Build<ConfirmWithdrawalPostRequest>()
            .With(x => x.HasConfirmed, false)
            .Create();

        // Act
        var actionResult = await _controller.ConfirmWithdrawal(request) as RedirectToActionResult;

        // Assert
        Assert.That(actionResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actionResult.ActionName, Is.EqualTo("Applications"));
            Assert.That(actionResult.RouteValues["encodedAccountId"], Is.EqualTo(request.EncodedAccountId));
        });
    }
}