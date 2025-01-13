using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers;

[Route("[controller]")]
public class LocationController(ILocationOrchestrator locationOrchestrator) : Controller
{
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetLocations([FromQuery] string searchTerm)
    {
        var viewModel = await locationOrchestrator.GetLocationsTypeAheadViewModel(searchTerm);
        return new JsonResult(viewModel);
    }
}