using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        private LocationController _controller;
        private Mock<ILocationOrchestrator> _orchestrator;
        private const string SearchTerm = "Search Term";

        [SetUp]
        public void Setup()
        {
            _orchestrator = new Mock<ILocationOrchestrator>();
            _controller = new LocationController(_orchestrator.Object);
        }

        [Test]
        public async Task GetLocations_GivenSearchTerm_ReturnsModel()
        {
            _orchestrator.Setup(o => o.GetLocationsTypeAheadViewModel(It.Is<string>(s => s == SearchTerm)))
                .ReturnsAsync(new LocationsTypeAheadViewModel
                {
                    Locations = new List<LocationTypeAheadViewModel>
                    {
                        new LocationTypeAheadViewModel
                        {
                            Name = SearchTerm
                        }
                    }
                });
            
            var result = await _controller.GetLocations(SearchTerm) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<LocationsTypeAheadViewModel>(result.Value);
        }
    }
}
