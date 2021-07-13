﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.LevyTransferMatching.Web.Authentication;

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
        
            return RedirectToAction("Apply", new { EncodedAccountId = encodedAccountId, EncodedPledgeId = encodedPledgeId });
        }

        [HideAccountNavigation(false)]
        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply")]
        public async Task<IActionResult> Apply(ApplicationRequest request)
        {
            return View(await _opportunitiesOrchestrator.GetApplyViewModel(request));
        }

        [HideAccountNavigation(false)]
        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpGet]
        [Route("accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply/contact-details")]
        public async Task<IActionResult> ContactDetails(ContactDetailsRequest contactDetailsRequest)
        {
            var viewModel = await _opportunitiesOrchestrator.GetContactDetailsViewModel(contactDetailsRequest);

            return View(viewModel);
        }

        [HideAccountNavigation(false)]
        [Authorize(Policy = PolicyNames.ManageAccount)]
        [HttpPost]
        [Route("accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply/contact-details")]
        public async Task<IActionResult> ContactDetails(ContactDetailsPostRequest contactDetailsPostRequest)
        {
            await _opportunitiesOrchestrator.UpdateCacheItem(contactDetailsPostRequest);

            return RedirectToAction(nameof(Apply), new { contactDetailsPostRequest.EncodedAccountId, contactDetailsPostRequest.EncodedPledgeId, contactDetailsPostRequest.CacheKey });
        }
    }
}