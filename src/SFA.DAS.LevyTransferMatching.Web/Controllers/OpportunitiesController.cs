﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Attributes;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using System;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [HideAccountNavigation(true)]
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
        public async Task<IActionResult> Detail(DetailRequest detailRequest)
        {
            var viewModel = await _opportunitiesOrchestrator.GetDetailViewModel((int)detailRequest.PledgeId);

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

        [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
        [Route("opportunities/{encodedPledgeId}/apply")]
        public async Task<IActionResult> SelectAccount(string encodedPledgeId)
        {
            var userId = _authenticationService.UserId;
            var encodedAccountId = await _opportunitiesOrchestrator.GetUserEncodedAccountId(userId);

            return RedirectToAction("Apply", new ApplicationRequest 
            { 
                CacheKey = Guid.NewGuid(),
                EncodedAccountId = encodedAccountId,
                EncodedPledgeId = encodedPledgeId
            });
        }

        [HideAccountNavigation(false)]
        [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
        [Route("/accounts/{encodedAccountId}/opportunities/{EncodedPledgeId}/apply")]
        public async Task<IActionResult> Apply(ApplicationRequest request)
        {
            return View(await _opportunitiesOrchestrator.GetApplyViewModel(request));
        }

        [HideAccountNavigation(false)]
        [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
        [Route("/accounts/{encodedAccountId}/opportunities/{EncodedPledgeId}/create/more-details")]
        public async Task<IActionResult> MoreDetails(MoreDetailsRequest request)
        {
            return View(await _opportunitiesOrchestrator.GetMoreDetailsViewModel(request));
        }

        [HideAccountNavigation(false)]
        [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
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
    }
}