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
        public void GET_Index_Returns_Expected_View_With_Expected_ViewModel()
        {
            // Arrange
            var encodedAccountId = _fixture.Create<string>();
            _orchestrator.Setup(x => x.GetIndexViewModel(encodedAccountId)).Returns(() => new IndexViewModel());

            // Act
            var viewResult = _pledgesController.Index(encodedAccountId) as ViewResult;
            var indexViewModel = viewResult?.Model as IndexViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(indexViewModel);
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
            var amountViewModel = viewResult?.Model as JobRoleViewModel;

            // Assert
            Assert.NotNull(viewResult);
            Assert.NotNull(amountViewModel);
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
    }
}