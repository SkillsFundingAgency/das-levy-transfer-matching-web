using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [Route("[controller]")]
    public class LocationController : Controller
    {
        private readonly ILocationOrchestrator _locationOrchestrator;

        public LocationController(ILocationOrchestrator locationOrchestrator)
        {
            _locationOrchestrator = locationOrchestrator;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetLocations([FromQuery] string searchTerm)
        {
            var viewModel = await _locationOrchestrator.GetLocationsTypeAheadViewModel(searchTerm);
            return new JsonResult(viewModel);
        }
    }
}
