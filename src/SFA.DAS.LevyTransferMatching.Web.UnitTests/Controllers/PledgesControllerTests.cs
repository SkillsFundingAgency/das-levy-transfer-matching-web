using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.LevyTransferMatching.Web.Controllers;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers;

[TestFixture]
public class PledgesControllerTests
{
    private Fixture _fixture;
    private PledgesController _pledgesController;
    private Mock<IPledgeOrchestrator> _orchestrator;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _orchestrator = new Mock<IPledgeOrchestrator>();
        _pledgesController = new PledgesController(_orchestrator.Object);
    }

    [TearDown]
    public void TearDown() => _pledgesController?.Dispose();

    [Test]
    public async Task GET_Pledges_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<PledgesRequest>();
        _orchestrator.Setup(x => x.GetPledgesViewModel(request)).ReturnsAsync(() => new PledgesViewModel());

        // Act
        var viewResult = await _pledgesController.Pledges(request) as ViewResult;
        var pledgesViewModel = viewResult?.Model as PledgesViewModel;

        // Assert
        viewResult.Should().NotBeNull();
        pledgesViewModel.Should().NotBeNull();
    }

    [Test]
    public void GET_Detail_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<DetailRequest>();
        _orchestrator.Setup(x => x.GetDetailViewModel(request)).Returns(() => new DetailViewModel());

        // Act
        var viewResult = _pledgesController.Detail(request) as ViewResult;
        var detailViewModel = viewResult?.Model as DetailViewModel;

        // Assert
        viewResult.Should().NotBeNull();
        detailViewModel.Should().NotBeNull();
    }

    [Test]
    public void GET_Close_Returns_Expected_View()
    {
        // Arrange
        var request = _fixture.Create<CloseRequest>();
        _orchestrator.Setup(x => x.GetCloseViewModel(request)).Returns(() => new CloseViewModel());

        // Act
        var viewResult = _pledgesController.Close(request) as ViewResult;
        var actualViewModel = viewResult?.Model as CloseViewModel;

        // Assert
        viewResult.Should().NotBeNull();
        actualViewModel.Should().NotBeNull();
    }

    [Test]
    public async Task POST_Close_Returns_Expected_Redirect_To_Pledges()
    {
        // Arrange
        var request = _fixture.Build<ClosePostRequest>()
            .With(x => x.HasConfirmed, true)
            .Create();

        _orchestrator.Setup(x => x.ClosePledge(request)).Returns(Task.CompletedTask);

        var mockTempData = new Mock<ITempDataDictionary>();
        _pledgesController.TempData = mockTempData.Object;

        // Act
        var actionResult = await _pledgesController.Close(request) as RedirectToActionResult;

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.ActionName.Should().Be("Pledges");
        actionResult.RouteValues["EncodedAccountId"].Should().Be(request.EncodedAccountId);
    }

    [Test]
    public async Task GET_Applications_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<ApplicationsRequest>();
        _orchestrator.Setup(x => x.GetApplications(request)).ReturnsAsync(new ApplicationsViewModel());

        // Act
        var viewResult = await _pledgesController.Applications(request) as ViewResult;
        var applicationsViewModel = viewResult?.Model as ApplicationsViewModel;

        // Assert
        viewResult.Should().NotBeNull();
        applicationsViewModel.Should().NotBeNull();
    }

    [Test]
    public async Task GET_Application_Returns_Expected_ViewModel()
    {
        var request = _fixture.Create<ApplicationRequest>();
        _orchestrator.Setup(x => x.GetApplicationViewModel(request, CancellationToken.None)).ReturnsAsync(new ApplicationViewModel());

        var viewResult = await _pledgesController.Application(request) as ViewResult;
        var viewModel = viewResult?.Model as ApplicationViewModel;

        viewResult.Should().NotBeNull();
        viewModel.Should().NotBeNull();
    }

    [Test]
    public async Task POST_Application_Approval_Redirects_To_ApplicationApprovalOptions()
    {
        var request = _fixture.Create<ApplicationPostRequest>();
        request.SelectedAction = ApplicationPostRequest.ApprovalAction.Approve;
        request.DisplayApplicationApprovalOptions = true;
        _orchestrator.Setup(x => x.SetApplicationOutcome(It.Is<ApplicationPostRequest>(r => r.AccountId == request.AccountId && r.ApplicationId == request.ApplicationId && r.PledgeId == request.PledgeId))).Returns(Task.CompletedTask);

        var redirectResult = await _pledgesController.Application(request) as RedirectToActionResult;

        redirectResult.Should().NotBeNull();
        redirectResult.ActionName.Should().Be("ApplicationApprovalOptions");
    }

    [Test]
    public async Task POST_Application_Approval_Redirects_To_ApplicationApproved()
    {
        var request = _fixture.Create<ApplicationPostRequest>();
        request.SelectedAction = ApplicationPostRequest.ApprovalAction.Approve;
        request.DisplayApplicationApprovalOptions = false;
        _orchestrator.Setup(x => x.SetApplicationOutcome(It.Is<ApplicationPostRequest>(r => r.AccountId == request.AccountId && r.ApplicationId == request.ApplicationId && r.PledgeId == request.PledgeId))).Returns(Task.CompletedTask);

        var redirectResult = await _pledgesController.Application(request) as RedirectToActionResult;

        redirectResult.Should().NotBeNull();
        redirectResult.ActionName.Should().Be("ApplicationApproved");
    }

    [Test]
    public async Task POST_Application_Rejection_Redirects_To_Applications()
    {
        var mockTempData = new Mock<ITempDataDictionary>();
        _pledgesController.TempData = mockTempData.Object;

        var request = _fixture.Create<ApplicationPostRequest>();
        request.SelectedAction = ApplicationPostRequest.ApprovalAction.Reject;
        _orchestrator.Setup(x => x.SetApplicationOutcome(It.Is<ApplicationPostRequest>(r => r.AccountId == request.AccountId && r.ApplicationId == request.ApplicationId && r.PledgeId == request.PledgeId))).Returns(Task.CompletedTask);

        var redirectResult = await _pledgesController.Application(request) as RedirectToActionResult;

        redirectResult.Should().NotBeNull();
        redirectResult.ActionName.Should().Be("Applications");
    }

    [Test]
    public void POST_Applications_Rejection_Redirects_To_Reject_Applications_Options()
    {
        // Arrange
        var request = _fixture.Create<ApplicationsPostRequest>();
        var listOfApplications = new List<string>
        {
            "9RMK6Y"
        };
        request.ApplicationsToReject = listOfApplications;

        // Act
        var redirectResult = _pledgesController.Applications(request) as RedirectToActionResult;

        // Assert
        redirectResult.Should().NotBeNull();
        redirectResult.ActionName.Should().Be("RejectApplications");
    }

    [Test]
    public async Task POST_Reject_Applications_Action_Redirects_To_Updated_List_Of_Applications_On_Confirm()
    {
        // Arrange
        var request = _fixture.Create<RejectApplicationsPostRequest>();
        var listOfApplications = new List<string>
        {
            "9RMK6Y"
        };
        request.ApplicationsToReject = listOfApplications;
        request.RejectConfirm = true;

        var mockTempData = new Mock<ITempDataDictionary>();
        _pledgesController.TempData = mockTempData.Object;

        _orchestrator.Setup(x => x.RejectApplications(request)).Returns(Task.CompletedTask);

        // Act
        var redirectResult = await _pledgesController.RejectApplications(request) as RedirectToActionResult;

        //Assert
        redirectResult.Should().NotBeNull();
        redirectResult.ActionName.Should().Be("Applications");
    }

    [Test]
    public async Task GET_ApplicationApproved_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<ApplicationApprovedRequest>();
        _orchestrator.Setup(x => x.GetApplicationApprovedViewModel(request)).ReturnsAsync(() => new ApplicationApprovedViewModel());

        // Act
        var viewResult = await _pledgesController.ApplicationApproved(request) as ViewResult;
        var applicationApprovedViewModel = viewResult?.Model as ApplicationApprovedViewModel;

        // Assert
        viewResult.Should().NotBeNull();
        applicationApprovedViewModel.Should().NotBeNull();
    }

    [Test]
    public async Task GET_ApplicationApprovalOptions_Returns_Expected_View_With_Expected_ViewModel()
    {
        // Arrange
        var request = _fixture.Create<ApplicationApprovalOptionsRequest>();
        _orchestrator.Setup(x => x.GetApplicationApprovalOptionsViewModel(request, CancellationToken.None)).ReturnsAsync(() => new ApplicationApprovalOptionsViewModel { IsApplicationPending = true });

        // Act
        var viewResult = await _pledgesController.ApplicationApprovalOptions(request) as ViewResult;
        var applicationApprovalOptionsViewModel = viewResult?.Model as ApplicationApprovalOptionsViewModel;

        // Assert
        viewResult.Should().NotBeNull();
        applicationApprovalOptionsViewModel.Should().NotBeNull();
    }

    [Test]
    public async Task GET_ApplicationApprovalOptions_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<ApplicationApprovalOptionsRequest>();
        _orchestrator.Setup(x => x.GetApplicationApprovalOptionsViewModel(request, CancellationToken.None)).ReturnsAsync(() => new ApplicationApprovalOptionsViewModel { IsApplicationPending = false });

        // Act
        var actionResult = await _pledgesController.ApplicationApprovalOptions(request) as RedirectToActionResult;

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.ActionName.Should().Be("Application");
        actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
        actionResult.RouteValues["encodedPledgeId"].Should().Be(request.EncodedPledgeId);
        actionResult.RouteValues["encodedApplicationId"].Should().Be(request.EncodedApplicationId);
    }

    [Test]
    public async Task POST_ApplicationApprovalOptions_Returns_Expected_Redirect()
    {
        // Arrange
        var request = _fixture.Create<ApplicationApprovalOptionsPostRequest>();

        // Act
        var actionResult = await _pledgesController.ApplicationApprovalOptions(request) as RedirectToActionResult;

        // Assert
        actionResult.Should().NotBeNull();
        actionResult.ActionName.Should().Be("ApplicationApproved");
        actionResult.RouteValues["encodedAccountId"].Should().Be(request.EncodedAccountId);
        actionResult.RouteValues["encodedPledgeId"].Should().Be(request.EncodedPledgeId);
        actionResult.RouteValues["encodedApplicationId"].Should().Be(request.EncodedApplicationId);
    }
}