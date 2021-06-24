using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System.Data;
using SFA.DAS.Authorization.Mvc.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
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

        [Route("opportunities/{encodedId}")]
        public async Task<IActionResult> Detail(string encodedId)
        {
            var viewModel = await _opportunitiesOrchestrator.GetDetailViewModel(encodedId);

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
        [Route("opportunities/{encodedId}")]
        public IActionResult ConfirmOpportunitySelection(string encodedId, DetailPostRequest detailPostRequest)
        {
            if (!detailPostRequest.HasConfirmed.HasValue)
            {
                throw new DataException($"{nameof(detailPostRequest.HasConfirmed)} should be validated and have a value.");
            }

            if (detailPostRequest.HasConfirmed.Value)
            {
                return RedirectToAction(nameof(RedirectToApply), new { encodedId });
            }
            else
            {
                // Go back.
                return RedirectToAction(nameof(Index));
            }
        }
        
        [DasAuthorize]
        [Route("opportunities/{encodedId}/apply")]
        public IActionResult RedirectToApply(string encodedId)
        {
            // TODO: To be auth'd at this point, and redirect to
            //       accounts/{encodedAccountId}/opportunities/{encodedId}.
            return Ok();
        }
    }
}
