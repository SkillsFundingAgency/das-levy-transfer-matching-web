using System.Collections.Generic;
using System.Linq;
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
            ViewBag.HideNav = false;
            return View(viewModel);
        }

        [Route("create")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            var viewModel = await _orchestrator.GetCreateViewModel(request);
            ViewBag.HideNav = false;
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
            ViewBag.HideNav = false;
            return View(viewModel);
        }

        [HttpPost]
        [Route("create/amount")]
        public async Task<IActionResult> Amount(AmountPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            ViewBag.HideNav = false;
            return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
        }

        [Route("create/sector")]
        public async Task<IActionResult> Sector(SectorRequest request)
        {
            var viewModel = await _orchestrator.GetSectorViewModel(request);
            ViewBag.HideNav = false;
            return View(viewModel);
        }

        [HttpPost]
        [Route("create/sector")]
        public async Task<IActionResult> Sector(SectorPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            ViewBag.HideNav = false;
            return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
        }

        [Route("create/jobrole")]
        public async Task<IActionResult> JobRole(JobRoleRequest request)
        {
            var viewModel = await _orchestrator.GetJobRoleViewModel(request);
            ViewBag.HideNav = false;
            return View(viewModel);
        }

        [HttpPost]
        [Route("create/jobrole")]
        public async Task<IActionResult> JobRole(JobRolePostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            ViewBag.HideNav = false;
            return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
        }

        [Route("create/level")]
        public async Task<IActionResult> Level(LevelRequest request)
        {
            var viewModel = await _orchestrator.GetLevelViewModel(request);
            ViewBag.HideNav = false;
            return View(viewModel);
        }

        [HttpPost]
        [Route("create/level")]
        public async Task<IActionResult> Level(LevelPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new CreateRequest() { EncodedAccountId = request.EncodedAccountId, CacheKey = request.CacheKey });
        }

        [Route("create/location")]
        public async Task<IActionResult> Location(LocationRequest request)
        {
            ViewBag.HideNav = false;

            var viewModel = await _orchestrator.GetLocationViewModel(request);
            return View(viewModel);
        }

        [HttpPost]
        [Route("create/location")]
        public async Task<IActionResult> Location(LocationPostRequest request)
        {
            ViewBag.HideNav = false;

            Dictionary<int, string> errors = await _orchestrator.ValidateLocations(request);
            if (errors.Any())
            {
                AddLocationErrorsToModelState(errors);
                return RedirectToAction("Location", request);
            }

            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new CreateRequest() { EncodedAccountId = request.EncodedAccountId, CacheKey = request.CacheKey });
        }

        private void AddLocationErrorsToModelState(Dictionary<int, string> errors)
        {
            foreach(var error in errors)
            {
                ModelState.AddModelError($"Locations[{error.Key}]", error.Value);
            }
        }
   }
}