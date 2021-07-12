using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("accounts/{encodedAccountId}/pledges")]
    public class PledgesController : Controller
    {
        private readonly IPledgeOrchestrator _orchestrator;

        public PledgesController(IPledgeOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("create/inform")]
        public IActionResult Inform(string encodedAccountId)
        {
            var viewModel = _orchestrator.GetInformViewModel(encodedAccountId);
            return View(viewModel);
        }

        [Route("create")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            var viewModel = await _orchestrator.GetCreateViewModel(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Submit(CreatePostRequest request)
        {
            var pledge = await _orchestrator.SubmitPledge(request);
            return RedirectToAction("Confirmation", new ConfirmationRequest { EncodedAccountId = request.EncodedAccountId, EncodedPledgeId = pledge });
        }

        [Route("create/amount")]
        public async  Task<IActionResult> Amount(AmountRequest request)
        {
            var viewModel = await _orchestrator.GetAmountViewModel(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("create/amount")]
        public async Task<IActionResult> Amount(AmountPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
        }

        [Route("create/sector")]
        public async Task<IActionResult> Sector(SectorRequest request)
        {
            var viewModel = await _orchestrator.GetSectorViewModel(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("create/sector")]
        public async Task<IActionResult> Sector(SectorPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
        }

        [Route("create/jobrole")]
        public async Task<IActionResult> JobRole(JobRoleRequest request)
        {
            var viewModel = await _orchestrator.GetJobRoleViewModel(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("create/jobrole")]
        public async Task<IActionResult> JobRole(JobRolePostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
        }

        [Route("create/level")]
        public async Task<IActionResult> Level(LevelRequest request)
        {
            var viewModel = await _orchestrator.GetLevelViewModel(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("create/level")]
        public async Task<IActionResult> Level(LevelPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new CreateRequest() { EncodedAccountId = request.EncodedAccountId, CacheKey = request.CacheKey });
        }

        [HttpGet]
        [Route("{EncodedPledgeId}/confirmation")]
        public async Task<IActionResult> Confirmation(ConfirmationRequest request)
        {
            return View(new ConfirmationViewModel() { EncodedAccountId = request.EncodedAccountId, EncodedPledgeId = request.EncodedPledgeId });
        }
    }
}