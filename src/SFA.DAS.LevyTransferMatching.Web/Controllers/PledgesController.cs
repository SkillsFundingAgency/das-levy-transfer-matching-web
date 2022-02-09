using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.ValidatorInterceptors;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [Authorize(Policy = PolicyNames.ViewAccount)]
    [Route("accounts/{encodedAccountId}/pledges")]
    public class PledgesController : Controller
    {
        private readonly IPledgeOrchestrator _orchestrator;

        public PledgesController(IPledgeOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("")]
        public async Task<IActionResult> Pledges(PledgesRequest request)
        {
            var viewModel = await _orchestrator.GetPledgesViewModel(request);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("create/inform")]
        public IActionResult Inform(string encodedAccountId)
        {
            var viewModel = _orchestrator.GetInformViewModel(encodedAccountId);
            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("{encodedPledgeId}/close")]
        public IActionResult Close(string encodedAccountId, string encodedPledgeId)
        {
            var viewModel = _orchestrator.GetCloseViewModel(encodedAccountId, encodedPledgeId);
            return View(viewModel);
        }

        [HttpPost]
        [Route("{encodedPledgeId}/close")]
        public async Task<IActionResult> Close(ClosePostRequest closePostRequest)
        {
            if (closePostRequest.HasConfirmed.Value)
            {
               await _orchestrator.ClosePledge(closePostRequest);
               
               TempData.AddFlashMessage("Transfer pledge closed", $"You closed the transfer pledge {closePostRequest.EncodedPledgeId}.", TempDataDictionaryExtensions.FlashMessageLevel.Success);
               return RedirectToAction(nameof(Pledges), new { EncodedAccountId = closePostRequest.EncodedAccountId });
            }
            return RedirectToAction(nameof(Applications), new { EncodedAccountId = closePostRequest.EncodedAccountId, EncodedPledgeId = closePostRequest.EncodedPledgeId });
        }

        [Route("{EncodedPledgeId}/detail")]
        public IActionResult Detail(DetailRequest request)
        {
            var viewModel = _orchestrator.GetDetailViewModel(request);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("create")]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            var viewModel = await _orchestrator.GetCreateViewModel(request);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Submit(CreatePostRequest request)
        {
            var pledge = await _orchestrator.SubmitPledge(request);
            return RedirectToAction("Confirmation", new ConfirmationRequest { EncodedAccountId = request.EncodedAccountId, EncodedPledgeId = pledge });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("create/amount")]
        public async  Task<IActionResult> Amount(AmountRequest request)
        {
            var viewModel = await _orchestrator.GetAmountViewModel(request);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("create/amount")]
        public async Task<IActionResult> Amount(AmountPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("create/sector")]
        public async Task<IActionResult> Sector(SectorRequest request)
        {
            var viewModel = await _orchestrator.GetSectorViewModel(request);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("create/sector")]
        public async Task<IActionResult> Sector(SectorPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("create/job-role")]
        public async Task<IActionResult> JobRole(JobRoleRequest request)
        {
            var viewModel = await _orchestrator.GetJobRoleViewModel(request);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("create/job-role")]
        public async Task<IActionResult> JobRole(JobRolePostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("create/level")]
        public async Task<IActionResult> Level(LevelRequest request)
        {
            var viewModel = await _orchestrator.GetLevelViewModel(request);
            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("create/level")]
        public async Task<IActionResult> Level(LevelPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);
            return RedirectToAction("Create", new CreateRequest() { EncodedAccountId = request.EncodedAccountId, CacheKey = request.CacheKey });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("create/location")]
        public async Task<IActionResult> Location(LocationRequest request)
        {
            var viewModel = await _orchestrator.GetLocationViewModel(request);

            return View(viewModel);
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("create/location")]
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
        [Route("create/location/select")]
        public async Task<IActionResult> LocationSelect(LocationSelectRequest request)
        {
            var viewModel = await _orchestrator.GetLocationSelectViewModel(request);

            return View(viewModel);
        }

        [Authorize]
        [Route("create/location/select")]
        [HttpPost]
        public async Task<IActionResult> LocationSelect([CustomizeValidator(Interceptor = typeof(LocationSelectPostRequestValidatorInterceptor))] LocationSelectPostRequest request)
        {
            await _orchestrator.UpdateCacheItem(request);

            return RedirectToAction("Create", new CreateRequest() { EncodedAccountId = request.EncodedAccountId, CacheKey = request.CacheKey });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpGet]
        [Route("{EncodedPledgeId}/confirmation")]
        public async Task<IActionResult> Confirmation(ConfirmationRequest request)
        {
            return View(new ConfirmationViewModel() { EncodedAccountId = request.EncodedAccountId, EncodedPledgeId = request.EncodedPledgeId });
        }

        private void AddLocationErrorsToModelState(Dictionary<int, string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError($"Locations[{error.Key}]", error.Value);
            }
        }

        [HttpGet]
        [Route("{encodedPledgeId}/applications")]
        public async Task<IActionResult> Applications(ApplicationsRequest request)
        {
            var response = await _orchestrator.GetApplications(request);

            return View(response);
        }

        [HttpGet]
        [Route("{encodedPledgeId}/applications/download")]
        public async Task<IActionResult> DownloadApplicationsCsv(ApplicationsRequest request)
        {
            var response = await _orchestrator.GetPledgeApplicationsDownloadModel(request);

            return new FileContentResult(response, "text/csv");
        }

        [HttpGet]
        [Route("{encodedPledgeId}/applications/{encodedApplicationId}")]
        public async Task<IActionResult> Application(ApplicationRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _orchestrator.GetApplicationViewModel(request, cancellationToken);

            if (response != null)
            {
                return View(response);
            }

            return NotFound();
        }

        [HttpPost]
        [Route("{encodedPledgeId}/applications/{encodedApplicationId}")]
        public async Task<IActionResult> Application(ApplicationPostRequest request)
        {
            if(request.DisplayApplicationApprovalOptions && request.SelectedAction == ApplicationPostRequest.ApprovalAction.Approve)
            {
                return RedirectToAction("ApplicationApprovalOptions", new { request.EncodedAccountId, request.EncodedPledgeId, request.EncodedApplicationId });
            }

            await _orchestrator.SetApplicationOutcome(request);

            if (request.SelectedAction == ApplicationPostRequest.ApprovalAction.Approve)
            {
                return RedirectToAction("ApplicationApproved", new { request.EncodedAccountId, request.EncodedPledgeId, request.EncodedApplicationId });
            }

            return RedirectToAction("Applications", new { request.EncodedAccountId, request.EncodedPledgeId, DisplayRejectedBanner = true, RejectedEmployerName = request.EmployerAccountName });
        }

        [HttpGet]
        [Route("{encodedPledgeId}/applications/{encodedApplicationId}/approved")]
        public async Task<IActionResult> ApplicationApproved(ApplicationApprovedRequest request)
        {
            var viewModel = await _orchestrator.GetApplicationApprovedViewModel(request);

            return View(viewModel);
        }

        [HttpGet]
        [Route("{encodedPledgeId}/applications/{encodedApplicationId}/approval-options")]
        public async Task<IActionResult> ApplicationApprovalOptions(ApplicationApprovalOptionsRequest request)
        {
            var viewModel = await _orchestrator.GetApplicationApprovalOptionsViewModel(request);

            if (viewModel.IsApplicationPending)
                return View(viewModel);
            else
                return RedirectToAction("Application", new { request.EncodedAccountId, request.EncodedPledgeId, request.EncodedApplicationId });
        }

        [HttpPost]
        [Route("{encodedPledgeId}/applications/{encodedApplicationId}/approval-options")]
        public async Task<IActionResult> ApplicationApprovalOptions(ApplicationApprovalOptionsPostRequest request)
        {
            await _orchestrator.SetApplicationApprovalOptions(request);
            return RedirectToAction("ApplicationApproved", new { request.EncodedAccountId, request.EncodedPledgeId, request.EncodedApplicationId });
        }
    }
}