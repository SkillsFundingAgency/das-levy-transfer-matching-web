using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.ValidatorInterceptors;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [Authorize(Policy = PolicyNames.ViewAccount)]
    [Route("accounts/{encodedAccountId}/pledges/create")]
    public class CreatePledgeController : Controller
    {
        private readonly ICreatePledgeOrchestrator _orchestrator;

        public CreatePledgeController(ICreatePledgeOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("inform")]
        public IActionResult Inform(string encodedAccountId)
        {
            var viewModel = _orchestrator.GetInformViewModel(encodedAccountId);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            var viewModel = await _orchestrator.GetCreateViewModel(request);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(CreatePostRequest request)
        {
            var pledge = await _orchestrator.CreatePledge(request);
            return RedirectToAction("Confirmation", new ConfirmationRequest { EncodedAccountId = request.EncodedAccountId, EncodedPledgeId = pledge });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("amount")]
        public async Task<IActionResult> Amount(AmountRequest request)
        {
            var viewModel = await _orchestrator.GetAmountViewModel(request);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("amount")]
        public async Task<IActionResult> Amount(AmountPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("sector")]
        public async Task<IActionResult> Sector(SectorRequest request)
        {
            var viewModel = await _orchestrator.GetSectorViewModel(request);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("sector")]
        public async Task<IActionResult> Sector(SectorPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("job-role")]
        public async Task<IActionResult> JobRole(JobRoleRequest request)
        {
            var viewModel = await _orchestrator.GetJobRoleViewModel(request);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("job-role")]
        public async Task<IActionResult> JobRole(JobRolePostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("level")]
        public async Task<IActionResult> Level(LevelRequest request)
        {
            var viewModel = await _orchestrator.GetLevelViewModel(request);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("level")]
        public async Task<IActionResult> Level(LevelPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new CreateRequest() { EncodedAccountId = request.EncodedAccountId, CacheKey = request.CacheKey });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("location")]
        public async Task<IActionResult> Location(LocationRequest request)
        {
            var viewModel = await _orchestrator.GetLocationViewModel(request);

            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("location")]
        public async Task<IActionResult> Location(LocationPostRequest request)
        {
            var multipleValidLocations = new Dictionary<int, IEnumerable<string>>();

            var errors = await _orchestrator.ValidateLocations(request, multipleValidLocations);

            if (errors.Any())
            {
                AddLocationErrorsToModelState(errors);

                return RedirectToAction(nameof(Location), new { request.EncodedAccountId, request.AccountId, request.CacheKey });
            }

            await _orchestrator.UpdateCacheItem(request);

            if (multipleValidLocations.Any() && !request.AllLocationsSelected)
            {
                // Then surface a view to allow them to select the correct
                // location, from a set of multiple valid locations
                return RedirectToAction(nameof(LocationSelect), new { request.EncodedAccountId, request.CacheKey });
            }

            return RedirectToAction("Create", new CreateRequest() { EncodedAccountId = request.EncodedAccountId, CacheKey = request.CacheKey });
        }

        [Authorize]
        [Route("location/select")]
        public async Task<IActionResult> LocationSelect(LocationSelectRequest request)
        {
            var viewModel = await _orchestrator.GetLocationSelectViewModel(request);

            return View(viewModel);
        }

        [Authorize]
        [Route("location/select")]
        [HttpPost]
        public async Task<IActionResult> LocationSelect([CustomizeValidator(Interceptor = typeof(LocationSelectPostRequestValidatorInterceptor))] LocationSelectPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);

            return RedirectToAction("Create", new CreateRequest() { EncodedAccountId = request.EncodedAccountId, CacheKey = request.CacheKey });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpGet]
        [Route("confirmation")]
        public IActionResult Confirmation(ConfirmationRequest request)
        {
            return View(new ConfirmationViewModel { EncodedAccountId = request.EncodedAccountId, EncodedPledgeId = request.EncodedPledgeId });
        }

        private void AddLocationErrorsToModelState(Dictionary<int, string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError($"Locations[{error.Key}]", error.Value);
            }
        }
    }
}
