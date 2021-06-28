using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
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

        [Route("opportunities/{encodedPledgeId}")]
        public async Task<IActionResult> Detail(string encodedPledgeId)
        {
            var viewModel = await _opportunitiesOrchestrator.GetDetailViewModel(encodedPledgeId);

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
        public IActionResult Detail(OpportunitiesPostRequest opportunitiesPostRequest)
        {
            if (opportunitiesPostRequest.HasConfirmed.Value)
            {
                return RedirectToAction(nameof(SelectAccount), new { opportunitiesPostRequest.EncodedPledgeId });
            }
            else
            {
                // Go back.
                return RedirectToAction(nameof(Index));
            }
        }
        
        [DasAuthorize]
        [Route("opportunities/{encodedPledgeId}/apply")]
        public async Task<IActionResult> SelectAccount(string encodedPledgeId)
        {
            var userId = _authenticationService.UserId;

            var encodedAccountId = await _opportunitiesOrchestrator.GetUserEncodedAccountId(userId);

            // TODO: Update to wire up to the actual controller (i.e.
            //       RedirectToAction) - which doesn't exist currently.
            return Redirect($"/accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply");
        }
    }
}
