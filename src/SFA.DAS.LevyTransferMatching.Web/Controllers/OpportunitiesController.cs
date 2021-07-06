using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [HideAccountNavigation(true)]
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

        [Route("opportunities/{encodedPledgeId}")]
        public async Task<IActionResult> Detail(DetailRequest detailRequest)
        {
            var viewModel = await _opportunitiesOrchestrator.GetDetailViewModel(detailRequest.PledgeId);

            if (viewModel != null)
            {
                return View(viewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("opportunities/{encodedPledgeId}")]
        public IActionResult Detail(DetailPostRequest detailPostRequest)
        {
            if (detailPostRequest.HasConfirmed.Value)
            {
                return RedirectToAction(nameof(SelectAccount), new { detailPostRequest.EncodedPledgeId });
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
        
        [Authorize]
        [Route("opportunities/{encodedPledgeId}/apply")]
        public async Task<IActionResult> SelectAccount(string encodedPledgeId)
        {
            var encodedAccountId = await _opportunitiesOrchestrator.GetUserEncodedAccountId();

            // TODO: Update to wire up to the actual controller (i.e.
            //       RedirectToAction) - which doesn't exist currently.
            return Redirect($"/accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply");
        }
    }
}