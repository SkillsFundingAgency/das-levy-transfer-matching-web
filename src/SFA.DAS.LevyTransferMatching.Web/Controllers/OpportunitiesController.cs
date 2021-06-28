using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [AllowAnonymous]
    [Route("opportunities")]
    [HideAccountNavigation(true)]
    public class OpportunitiesController : Controller
    {
        private readonly IOpportunitiesOrchestrator _searchFundingOrchestrator;

        public OpportunitiesController(IOpportunitiesOrchestrator searchFundingOrchestrator)
        {
            _searchFundingOrchestrator = searchFundingOrchestrator;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _searchFundingOrchestrator.GetIndexViewModel();
            return View(viewModel);
        }
    }
}
