using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private Mock<IApplicationsOrchestrator> _orchestrator;
        private ApplicationsController _controller;

        [SetUp]
        public void Setup()
        {
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
    }
}
