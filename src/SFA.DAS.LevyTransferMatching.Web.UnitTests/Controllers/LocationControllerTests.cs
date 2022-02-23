using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Controllers;
using SFA.DAS.LevyTransferMatching.Web.Models.Location;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Controllers
{
    public class LocationControllerTests
    {
        private Mock<ILocationOrchestrator> _orchestrator;
        private LocationController _controller;

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<ILocationOrchestrator>();
            _controller = new LocationController(_orchestrator.Object);
        }

        [Test]
        public async Task GET_GetLocations_ReturnsJsonResult()
        {
            _orchestrator.Setup(o =>
                    o.GetLocationsTypeAheadViewModel(It.IsAny<string>()))
                .ReturnsAsync(new LocationsTypeAheadViewModel
                {
                    Locations = new List<LocationTypeAheadViewModel>()
                });

            var result = await _controller.GetLocations("test") as JsonResult;
            Assert.IsNotNull(result);
        }
    }
}
