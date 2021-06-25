using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System.Data;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Authentication;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    public class OpportunitiesController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IOpportunitiesOrchestrator _opportunitiesOrchestrator;

        public OpportunitiesController(IAuthenticationService authenticationService, IOpportunitiesOrchestrator searchFundingOrchestrator)
        {
            _authenticationService = authenticationService;
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
            // TODO: Move into Orchestrator
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
        public async Task<IActionResult> RedirectToApply(string encodedId)
        {
            var userId = _authenticationService.UserId;

            var encodedAccountId = await _opportunitiesOrchestrator.GetUserEncodedAccountId(userId);

            // TODO: Update to wire up to the actual controller (i.e.
            //       RedirectToAction) - which doesn't exist currently.
            return Redirect($"/accounts/{encodedAccountId}/opportunities/{encodedId}/apply");
        }
    }
}
