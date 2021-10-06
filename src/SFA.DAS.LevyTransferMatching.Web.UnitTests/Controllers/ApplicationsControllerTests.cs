using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Controllers;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers
{
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
            Assert.AreEqual("ID", actual.EncodedAccountId);
        }

        [Test]
        public async Task GET_Application_ApplicationExists_ReturnsViewAndModel()
        {
            // Arrange
            var request = _fixture.Create<ApplicationRequest>();
            _orchestrator
                .Setup(x => x.GetApplication(request))
                .ReturnsAsync(new ApplicationViewModel());

            // Act
            var actionResult = await _controller.Application(request);
            var viewResult = actionResult as ViewResult;
            var model = viewResult.Model;
            var applicationViewModel = model as ApplicationViewModel;

            // Assert
            Assert.NotNull(actionResult);
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.NotNull(applicationViewModel);
        }

        [Test]
        public async Task GET_Application_ApplicationDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var request = _fixture.Create<ApplicationRequest>();
            _orchestrator
                .Setup(x => x.GetApplication(request))
                .ReturnsAsync((ApplicationViewModel)null);

            // Act
            var actionResult = await _controller.Application(request);
            var notFoundResult = actionResult as NotFoundResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.NotNull(notFoundResult);
        }

        [Test]
        public async Task GET_Accepted_ApplicationExists_ReturnsViewAndModel()
        {
            // Arrange
            var request = _fixture.Create<AcceptedRequest>();
            _orchestrator
                .Setup(x => x.GetAcceptedViewModel(request))
                .ReturnsAsync(new AcceptedViewModel());

            // Act
            var actionResult = await _controller.Accepted(request);
            var viewResult = actionResult as ViewResult;
            var model = viewResult.Model;
            var acceptedViewModel = model as AcceptedViewModel;

            // Assert
            Assert.NotNull(actionResult);
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.NotNull(acceptedViewModel);
        }

        [Test]
        public async Task GET_Accepted_ApplicationDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var request = _fixture.Create<AcceptedRequest>();
            _orchestrator
                .Setup(x => x.GetAcceptedViewModel(request))
                .ReturnsAsync((AcceptedViewModel)null);

            // Act
            var actionResult = await _controller.Accepted(request);
            var notFoundResult = actionResult as NotFoundResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.NotNull(notFoundResult);
        }
    }
}
