using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Controllers;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers
{
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

        [Test]
        public void GET_Inform_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var encodedAccountId = _fixture.Create<string>();
            _orchestrator.Setup(x => x.GetInformViewModel(encodedAccountId)).Returns(() => new InformViewModel());

            // Act
            var viewResult = _pledgesController.Inform(encodedAccountId) as ViewResult;
            var indexViewModel = viewResult?.Model as InformViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(indexViewModel);
        }

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
            Assert.NotNull(viewResult);
            Assert.NotNull(pledgesViewModel);
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
            Assert.NotNull(viewResult);
            Assert.NotNull(detailViewModel);
        }

        [Test]
        public async Task GET_Create_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var request = _fixture.Create<CreateRequest>();
            _orchestrator.Setup(x => x.GetCreateViewModel(request)).ReturnsAsync(() => new CreateViewModel());

            // Act
            var viewResult = await _pledgesController.Create(request) as ViewResult;
            var createViewModel = viewResult?.Model as CreateViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(createViewModel);
        }

        [Test]
        public async Task GET_Amount_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var request = _fixture.Create<AmountRequest>();
            _orchestrator.Setup(x => x.GetAmountViewModel(request)).ReturnsAsync(() => new AmountViewModel());

            // Act
            var viewResult = await _pledgesController.Amount(request) as ViewResult;
            var amountViewModel = viewResult?.Model as AmountViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(amountViewModel);
        }

        [Test]
        public async Task POST_Amount_Returns_Expected_Redirect()
        {
            // Arrange
            var request = _fixture.Create<AmountPostRequest>();

            // Act
            var actionResult = await _pledgesController.Amount(request) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("Create", actionResult.ActionName);
            Assert.AreEqual(request.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
        }

        [Test]
        public async Task GET_Sector_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var request = _fixture.Create<SectorRequest>();
            _orchestrator.Setup(x => x.GetSectorViewModel(request)).ReturnsAsync(() => new SectorViewModel());

            // Act
            var viewResult = await _pledgesController.Sector(request) as ViewResult;
            var amountViewModel = viewResult?.Model as SectorViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(amountViewModel);
        }

        [Test]
        public async Task POST_Sector_Returns_Expected_Redirect()
        {
            // Arrange
            var request = _fixture.Create<SectorPostRequest>();

            // Act
            var actionResult = await _pledgesController.Sector(request) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("Create", actionResult.ActionName);
            Assert.AreEqual(request.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
        }

        [Test]
        public async Task GET_Level_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var request = _fixture.Create<LevelRequest>();
            _orchestrator.Setup(x => x.GetLevelViewModel(request)).ReturnsAsync(() => new LevelViewModel());

            // Act
            var viewResult = await _pledgesController.Level(request) as ViewResult;
            var amountViewModel = viewResult?.Model as LevelViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(amountViewModel);
        }

        [Test]
        public async Task POST_Level_Returns_Expected_Redirect()
        {
            // Arrange
            var request = _fixture.Create<LevelViewModel>();

            // Act
            var actionResult = await _pledgesController.Level(request) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("Create", actionResult.ActionName);
            Assert.AreEqual(request.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
        }

        [Test]
        public async Task GET_JobRole_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var request = _fixture.Create<JobRoleRequest>();
            _orchestrator.Setup(x => x.GetJobRoleViewModel(request)).ReturnsAsync(() => new JobRoleViewModel());

            // Act
            var viewResult = await _pledgesController.JobRole(request) as ViewResult;
            var jobRoleViewModel = viewResult?.Model as JobRoleViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(jobRoleViewModel);
        }

        [Test]
        public async Task POST_JobRole_Returns_Expected_Redirect()
        {
            // Arrange
            var request = _fixture.Create<JobRolePostRequest>();

            // Act
            var actionResult = await _pledgesController.JobRole(request) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("Create", actionResult.ActionName);
            Assert.AreEqual(request.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
        }

        [Test]
        public async Task GET_Location_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var request = _fixture.Create<LocationRequest>();
            _orchestrator.Setup(x => x.GetLocationViewModel(request)).ReturnsAsync(() => new LocationViewModel());

            // Act
            var viewResult = await _pledgesController.Location(request) as ViewResult;
            var locationViewModel = viewResult?.Model as LocationViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(locationViewModel);
        }

        [Test]
        public async Task POST_Location_Returns_Expected_Redirect_To_Create_With_Valid_Location()
        {
            // Arrange
            var request = _fixture.Create<LocationPostRequest>();
            _orchestrator
                .Setup(x => x.ValidateLocations(It.IsAny<LocationPostRequest>(), It.IsAny<IDictionary<int, IEnumerable<string>>>()))
                .ReturnsAsync(new Dictionary<int, string>());

            // Act
            var actionResult = await _pledgesController.Location(request) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("Create", actionResult.ActionName);
            Assert.AreEqual(request.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
            Assert.AreEqual(request.CacheKey, actionResult.RouteValues["cacheKey"]);
        }

        [Test]
        public async Task POST_Location_Returns_Expected_Redirect_To_Location_With_Invalid_Location()
        {
            // Arrange
            var request = _fixture.Create<LocationPostRequest>();
            _orchestrator
                .Setup(x => x.ValidateLocations(It.IsAny<LocationPostRequest>(), It.IsAny<IDictionary<int, IEnumerable<string>>>()))
                .ReturnsAsync(new Dictionary<int, string>() { { 1, "Error Message" } });

            // Act
            var actionResult = await _pledgesController.Location(request) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("Location", actionResult.ActionName);
            Assert.AreEqual(request.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
            Assert.AreEqual(request.CacheKey, actionResult.RouteValues["cacheKey"]);
        }

        [Test]
        public async Task POST_Location_Returns_Expected_Redirect_To_LocationSelect()
        {
            // Arrange
            var request = _fixture
                .Build<LocationPostRequest>()
                .With(x => x.AllLocationsSelected, false)
                .Create();

            Action<LocationPostRequest, IDictionary<int, IEnumerable<string>>> validateCallback =
                (x, y) =>
                {
                    y.Add(_fixture.Create<KeyValuePair<int, IEnumerable<string>>>());
                };
            
            _orchestrator
                .Setup(x => x.ValidateLocations(It.IsAny<LocationPostRequest>(), It.IsAny<IDictionary<int, IEnumerable<string>>>()))
                .Callback(validateCallback)
                .ReturnsAsync(new Dictionary<int, string>());

            // Act
            var actionResult = await _pledgesController.Location(request) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("LocationSelect", actionResult.ActionName);
            Assert.AreEqual(request.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
            Assert.AreEqual(request.CacheKey, actionResult.RouteValues["cacheKey"]);
        }

        [Test]
        public async Task POST_LocationSelect_AllLocationsSelected_Redirect_To_Create()
        {
            // Arrange
            var request = _fixture
                .Build<LocationPostRequest>()
                .With(x => x.AllLocationsSelected, true)
                .Create();

            _orchestrator
                .Setup(x => x.ValidateLocations(It.IsAny<LocationPostRequest>(), It.IsAny<IDictionary<int, IEnumerable<string>>>()))
                .ReturnsAsync(new Dictionary<int, string>());

            // Act
            var actionResult = await _pledgesController.Location(request) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("Create", actionResult.ActionName);
            Assert.AreEqual(request.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
            Assert.AreEqual(request.CacheKey, actionResult.RouteValues["cacheKey"]);
        }

        [Test]
        public async Task GET_LocationSelect_Returns_View_With_Expected_ViewModel()
        {
            // Arrange
            var request = _fixture.Create<LocationSelectRequest>();
            var expectedViewModel = _fixture.Create<LocationSelectViewModel>();

            _orchestrator
                .Setup(x => x.GetLocationSelectViewModel(It.IsAny<LocationSelectRequest>()))
                .ReturnsAsync(expectedViewModel);

            // Act
            var actionResult = await _pledgesController.LocationSelect(request);
            var viewResult = actionResult as ViewResult;
            var model = viewResult.Model;
            var actualViewModel = model as LocationSelectViewModel;

            // Assert
            Assert.NotNull(actionResult);
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.NotNull(actualViewModel);
            Assert.AreEqual(expectedViewModel, actualViewModel);
        }

        [Test]
        public async Task POST_LocationSelect_Returns_View_With_Expected_Redirect()
        {
            // Arrange
            var request = _fixture.Create<LocationSelectPostRequest>();

            // Act
            var actionResult = await _pledgesController.LocationSelect(request);
            var redirectToAction = actionResult as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.NotNull(redirectToAction);
            Assert.AreEqual(nameof(PledgesController.Create), redirectToAction.ActionName);
        }

        [Test]
        public async Task POST_Create_Returns_Expected_Redirect()
        {
            // Arrange
            var request = _fixture.Create<CreatePostRequest>();

            // Act
            var actionResult = await _pledgesController.Submit(request) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("Confirmation", actionResult.ActionName);
            Assert.AreEqual(request.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
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
            Assert.NotNull(viewResult);
            Assert.NotNull(applicationsViewModel);
        }

        [Test]
        public async Task GET_Application_Returns_Expected_ViewModel()
        {
            var request = _fixture.Create<ApplicationRequest>();
            _orchestrator.Setup(x => x.GetApplicationViewModel(request, CancellationToken.None)).ReturnsAsync(new ApplicationViewModel());

            var viewResult = await _pledgesController.Application(request) as ViewResult;
            var viewModel = viewResult?.Model as ApplicationViewModel;

            Assert.NotNull(viewResult);
            Assert.NotNull(viewModel);
        }

        [Test]
        public async Task POST_Application_Approval_Redirects_To_ApplicationApprovalOptions()
        {
            var request = _fixture.Create<ApplicationPostRequest>();
            request.SelectedAction = ApplicationPostRequest.ApprovalAction.Approve;
            request.DisplayApplicationApprovalOptions = true;
            _orchestrator.Setup(x => x.SetApplicationOutcome(It.Is<ApplicationPostRequest>(r => r.AccountId == request.AccountId && r.ApplicationId == request.ApplicationId && r.PledgeId == request.PledgeId))).Returns(Task.CompletedTask);

            var redirectResult = await _pledgesController.Application(request) as RedirectToActionResult;

            Assert.NotNull(redirectResult);
            Assert.AreEqual("ApplicationApprovalOptions", redirectResult.ActionName);
        }

        [Test]
        public async Task POST_Application_Approval_Redirects_To_ApplicationApproved()
        {
            var request = _fixture.Create<ApplicationPostRequest>();
            request.SelectedAction = ApplicationPostRequest.ApprovalAction.Approve;
            request.DisplayApplicationApprovalOptions = false;
            _orchestrator.Setup(x => x.SetApplicationOutcome(It.Is<ApplicationPostRequest>(r => r.AccountId == request.AccountId && r.ApplicationId == request.ApplicationId && r.PledgeId == request.PledgeId))).Returns(Task.CompletedTask);

            var redirectResult = await _pledgesController.Application(request) as RedirectToActionResult;

            Assert.NotNull(redirectResult);
            Assert.AreEqual("ApplicationApproved", redirectResult.ActionName);
        }

        [Test]
        public async Task POST_Application_Rejection_Redirects_To_Applications()
        {
            var request = _fixture.Create<ApplicationPostRequest>();
            request.SelectedAction = ApplicationPostRequest.ApprovalAction.Reject;
            _orchestrator.Setup(x => x.SetApplicationOutcome(It.Is<ApplicationPostRequest>(r => r.AccountId == request.AccountId && r.ApplicationId == request.ApplicationId && r.PledgeId == request.PledgeId))).Returns(Task.CompletedTask);

            var redirectResult = await _pledgesController.Application(request) as RedirectToActionResult;

            Assert.NotNull(redirectResult);
            Assert.AreEqual("Applications", redirectResult.ActionName);
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
            Assert.NotNull(viewResult);
            Assert.NotNull(applicationApprovedViewModel);
        }

        [Test]
        public void GET_ApplicationApprovalOptions_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var request = _fixture.Create<ApplicationApprovalOptionsRequest>();
            _orchestrator.Setup(x => x.GetApplicationApprovalOptionsViewModel(request, CancellationToken.None)).Returns(() => new ApplicationApprovalOptionsViewModel());

            // Act
            var viewResult = _pledgesController.ApplicationApprovalOptions(request) as ViewResult;
            var applicationApprovalOptionsViewModel = viewResult?.Model as ApplicationApprovalOptionsViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(applicationApprovalOptionsViewModel);
        }

        [Test]
        public async Task POST_ApplicationApprovalOptions_Returns_Expected_Redirect()
        {
            // Arrange
            var request = _fixture.Create<ApplicationApprovalOptionsPostRequest>();

            // Act
            var actionResult = await _pledgesController.ApplicationApprovalOptions(request) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("ApplicationApproved", actionResult.ActionName);
            Assert.AreEqual(request.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
            Assert.AreEqual(request.EncodedPledgeId, actionResult.RouteValues["encodedPledgeId"]);
            Assert.AreEqual(request.EncodedApplicationId, actionResult.RouteValues["encodedApplicationId"]);
        }
    }
}