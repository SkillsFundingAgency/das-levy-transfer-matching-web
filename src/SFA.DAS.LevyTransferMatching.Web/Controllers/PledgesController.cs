using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
    [Route("accounts/{EncodedAccountId}/pledges")]
    public class PledgesController : Controller
    {
        private readonly ILinkGenerator _linkGenerator;
        private readonly IPledgeOrchestrator _orchestrator;

        public PledgesController(ILinkGenerator linkGenerator, IPledgeOrchestrator orchestrator)
        {
            _linkGenerator = linkGenerator;
            _orchestrator = orchestrator;
        }

        public IActionResult Index(string encodedAccountId)
        {
            var viewModel = _orchestrator.GetIndexViewModel(encodedAccountId);
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
            await _orchestrator.SubmitPledge(request);
            return Redirect(_linkGenerator.AccountsLink($"accounts/{request.EncodedAccountId}/transfers"));
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
   }
}