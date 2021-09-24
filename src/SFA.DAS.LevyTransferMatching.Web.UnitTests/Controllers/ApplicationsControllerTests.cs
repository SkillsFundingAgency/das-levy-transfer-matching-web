using System;
using System.Collections.Generic;
using System.Text;
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

            var result = await _controller.GetApplications(new GetApplicationsRequest()) as ViewResult;

            var actual = result.Model as GetApplicationsViewModel;
            Assert.AreEqual("ID", actual.EncodedAccountId);
        }

        [Test]
        public async Task GET_ApplicationStatus_ApplicationExists_ReturnsViewAndModel()
        {
            // Arrange
            var request = _fixture.Create<ApplicationStatusRequest>();
            _orchestrator
                .Setup(x => x.GetApplicationStatusViewModel(request))
                .ReturnsAsync(new ApplicationStatusViewModel());

            // Act
            var actionResult = await _controller.ApplicationStatus(request);
            var viewResult = actionResult as ViewResult;
            var model = viewResult.Model;
            var applicationStatusViewModel = model as ApplicationStatusViewModel;

            // Assert
            Assert.NotNull(actionResult);
            Assert.NotNull(viewResult);
            Assert.NotNull(model);
            Assert.NotNull(applicationStatusViewModel);
        }

        [Test]
        public async Task GET_ApplicationStatus_ApplicationDoesntExist_ReturnsNotFound()
        {
            // Arrange
            var request = _fixture.Create<ApplicationStatusRequest>();
            _orchestrator
                .Setup(x => x.GetApplicationStatusViewModel(request))
                .ReturnsAsync((ApplicationStatusViewModel)null);

            // Act
            var actionResult = await _controller.ApplicationStatus(request);
            var notFoundResult = actionResult as NotFoundResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.NotNull(notFoundResult);
        }
    }
}
