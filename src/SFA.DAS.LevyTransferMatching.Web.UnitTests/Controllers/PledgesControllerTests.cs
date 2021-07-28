using System.Collections.Generic;
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
        public async Task GET_MyPledges_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var request = _fixture.Create<MyPledgesRequest>();
            _orchestrator.Setup(x => x.GetMyPledgesViewModel(request)).ReturnsAsync(() => new MyPledgesViewModel());

            // Act
            var viewResult = await _pledgesController.MyPledges(request) as ViewResult;
            var myPledgesViewModel = viewResult?.Model as MyPledgesViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(myPledgesViewModel);
        }

        [Test]
        public async Task GET_Detail_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var request = _fixture.Create<DetailRequest>();
            _orchestrator.Setup(x => x.GetDetailViewModel(request)).ReturnsAsync(() => new DetailViewModel());

            // Act
            var viewResult = await _pledgesController.Detail(request) as ViewResult;
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
        public async Task POST_Location_Returns_Expected_Redirect_With_Valid_Location()
        {
            // Arrange
            var request = _fixture.Create<LocationPostRequest>();
            _orchestrator.Setup(x => x.ValidateLocations(It.IsAny<LocationPostRequest>())).Returns(Task.FromResult(new Dictionary<int, string>()));

            // Act
            var actionResult = await _pledgesController.Location(request) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("Create", actionResult.ActionName);
            Assert.AreEqual(request.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
        }

        [Test]
        public async Task POST_Location_Returns_Expected_Redirect_With_Invalid_Location()
        {
            // Arrange
            var request = _fixture.Create<LocationPostRequest>();
            _orchestrator.Setup(x => x.ValidateLocations(It.IsAny<LocationPostRequest>())).Returns(Task.FromResult(new Dictionary<int, string>() { { 1, "Error Message" } }));

            // Act
            var actionResult = await _pledgesController.Location(request) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual("Location", actionResult.ActionName);
            Assert.AreEqual(request.EncodedAccountId, actionResult.RouteValues["encodedAccountId"]);
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
    }
}