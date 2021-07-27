using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Attributes;
using System;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Validators;
using FluentValidation.AspNetCore;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [HideAccountNavigation(true)]
    public class OpportunitiesController : Controller
    {
        private readonly IOpportunitiesOrchestrator _opportunitiesOrchestrator;

        public OpportunitiesController(IOpportunitiesOrchestrator opportunitiesOrchestrator)
        {
            _opportunitiesOrchestrator = opportunitiesOrchestrator;
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

            return RedirectToAction("Apply", new 
            { 
                CacheKey = Guid.NewGuid(),
                EncodedAccountId = encodedAccountId,
                EncodedPledgeId = encodedPledgeId
            });
        }

        [HideAccountNavigation(false)]
        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("/accounts/{encodedAccountId}/opportunities/{EncodedPledgeId}/apply")]
        public async Task<IActionResult> Apply(ApplicationRequest request)
        {
            return View(await _opportunitiesOrchestrator.GetApplyViewModel(request));
        }

        [HideAccountNavigation(false)]
        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("/accounts/{encodedAccountId}/opportunities/{EncodedPledgeId}/create/more-details")]
        public async Task<IActionResult> MoreDetails(MoreDetailsRequest request)
        {
            return View(await _opportunitiesOrchestrator.GetMoreDetailsViewModel(request));
        }

        [HideAccountNavigation(false)]
        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("/accounts/{encodedAccountId}/opportunities/{EncodedPledgeId}/create/more-details")]
        [HttpPost]
        public async Task<IActionResult> MoreDetails(MoreDetailsPostRequest request)
        {
            await _opportunitiesOrchestrator.UpdateCacheItem(request);
            return RedirectToAction("Apply", new ApplicationRequest 
            { 
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                CacheKey = request.CacheKey
            });
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("/accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/create/application-details")]
        public async Task<IActionResult> ApplicationDetails(ApplicationDetailsRequest request)
        {
            return View(await _opportunitiesOrchestrator.GetApplicationViewModel(request));
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("/accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/create/application-details")]
        public async Task<IActionResult> ApplicationDetails(ApplicationDetailsPostRequest request)
        {
            return RedirectToAction("Apply", await _opportunitiesOrchestrator.PostApplicationViewModel(request));
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("/accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/create/sector")]
        public async Task<IActionResult> Sector(SectorRequest request)
        {
            return View(await _opportunitiesOrchestrator.GetSectorViewModel(request));
        }

        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("/accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/create/sector")]
        public async Task<IActionResult> Sector([FromServices] AsyncValidator<SectorPostRequest> validator, SectorPostRequest request)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState, "");

                return RedirectToAction("Sector", new SectorRequest
                {
                    EncodedAccountId = request.EncodedAccountId,
                    EncodedPledgeId = request.EncodedPledgeId,
                    CacheKey = request.CacheKey
                });
            }

            await _opportunitiesOrchestrator.UpdateCacheItem(request);

            return RedirectToAction("Apply", new ApplicationRequest
            {
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                CacheKey = request.CacheKey
            });
        }
    }
}