using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [AllowAnonymous]
    [Route("opportunities")]
    public class OpportunitiesController : Controller
    {
        private readonly IOpportunitiesOrchestrator _opportunitiesOrchestrator;

        public OpportunitiesController(IOpportunitiesOrchestrator searchFundingOrchestrator)
        {
            _opportunitiesOrchestrator = searchFundingOrchestrator;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _opportunitiesOrchestrator.GetIndexViewModel();
            return View(viewModel);
        }
    }
}
