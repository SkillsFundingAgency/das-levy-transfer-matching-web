﻿using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.Validators;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers;

[HideAccountNavigation(true)]
public class OpportunitiesController(IOpportunitiesOrchestrator opportunitiesOrchestrator) : Controller
{
    [Route("opportunities", Name = "opportunities")]
    public async Task<IActionResult> Index(IndexRequest request)
    {
        if (string.IsNullOrEmpty(request.CommaSeparatedSectors) && request.Sectors != null)
        {
            request.CommaSeparatedSectors = request.PopulateCommaSeparatedSectorsFromSectors();
        }
        else
        {
            request.Sectors = request.GetSectorsList();          
        }

        if (ModelState.ContainsKey(nameof(request.CommaSeparatedSectors)))
        {
            ModelState.SetModelValue(nameof(request.Sectors), new ValueProviderResult(request.CommaSeparatedSectors));
        }

        var viewModel = await opportunitiesOrchestrator.GetIndexViewModel(request);
        return View(viewModel);
    }

    [Route("opportunities/{encodedPledgeId}")]
    public async Task<IActionResult> Detail(DetailRequest detailRequest)
    {
        var viewModel = await opportunitiesOrchestrator.GetDetailViewModel(detailRequest);

        if (viewModel != null)
        {
            return View(viewModel);
        }

        return NotFound();
    }

    [HttpPost]
    [Route("opportunities/{encodedPledgeId}")]
    public IActionResult Detail(DetailPostRequest detailPostRequest)
    {
        return detailPostRequest.HasConfirmed.Value
            ? RedirectToAction(nameof(SelectAccount), new { EncodedOpportunityId = detailPostRequest.EncodedPledgeId })
            : RedirectToAction(nameof(Index));
    }

    [HideAccountNavigation(false, hideNavigationLinks: true)]
    [Authorize(Policy = PolicyNames.IsAuthenticated)]
    [Route("opportunities/{encodedOpportunityId}/select-account")]
    public async Task<IActionResult> SelectAccount(SelectAccountRequest selectAccountRequest)
    {
        var viewModel = await opportunitiesOrchestrator.GetSelectAccountViewModel(selectAccountRequest);

        if (viewModel.Accounts.Count() != 1)
        {
            return View(viewModel);
        }

        return RedirectToAction("Apply", new
        {
            CacheKey = Guid.NewGuid(),
            EncodedAccountId = viewModel.Accounts.Single().EncodedAccountId,
            EncodedPledgeId = selectAccountRequest.EncodedOpportunityId,
        });
    }

    [HideAccountNavigation(false)]
    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply")]
    public async Task<IActionResult> Apply(ApplicationRequest request)
    {
        return View(await opportunitiesOrchestrator.GetApplyViewModel(request));
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("/accounts/{encodedAccountId}/opportunities/{EncodedPledgeId}/apply")]
    public async Task<IActionResult> Apply(ApplyPostRequest request)
    {
        await opportunitiesOrchestrator.SubmitApplication(request);

        return RedirectToAction("Confirmation", new
        {
            request.EncodedAccountId,
            request.EncodedPledgeId
        });
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HideAccountNavigation(false)]
    [Route("/accounts/{encodedAccountId}/opportunities/{EncodedPledgeId}/apply/confirmation")]
    public async Task<IActionResult> Confirmation(ConfirmationRequest request)
    {
        return View(await opportunitiesOrchestrator.GetConfirmationViewModel(request));
    }

    [HideAccountNavigation(false)]
    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("/accounts/{encodedAccountId}/opportunities/{EncodedPledgeId}/apply/more-details")]
    public async Task<IActionResult> MoreDetails(MoreDetailsRequest request)
    {
        return View(await opportunitiesOrchestrator.GetMoreDetailsViewModel(request));
    }

    [HideAccountNavigation(false)]
    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("/accounts/{encodedAccountId}/opportunities/{EncodedPledgeId}/apply/more-details")]
    [HttpPost]
    public async Task<IActionResult> MoreDetails(MoreDetailsPostRequest request)
    {
        await opportunitiesOrchestrator.UpdateCacheItem(request);
        return RedirectToAction("Apply", new ApplicationRequest
        {
            EncodedAccountId = request.EncodedAccountId,
            EncodedPledgeId = request.EncodedPledgeId,
            CacheKey = request.CacheKey
        });
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("/accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply/application-details")]
    public async Task<IActionResult> ApplicationDetails(ApplicationDetailsRequest request)
    {
        return View(await opportunitiesOrchestrator.GetApplicationViewModel(request));
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("/accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply/application-details")]
    public async Task<IActionResult> ApplicationDetails(
        [FromServices] AsyncValidator<ApplicationDetailsPostRequest> validator, ApplicationDetailsPostRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState, string.Empty);

            return RedirectToAction("ApplicationDetails", new ApplicationDetailsRequest()
            {
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                CacheKey = request.CacheKey
            });
        }

        return RedirectToAction("Apply", await opportunitiesOrchestrator.PostApplicationViewModel(request));
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("/accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply/sector")]
    public async Task<IActionResult> Sector(SectorRequest request)
    {
        return View(await opportunitiesOrchestrator.GetSectorViewModel(request));
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("/accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply/sector")]
    public async Task<IActionResult> Sector(SectorPostRequest request)
    {
        await opportunitiesOrchestrator.UpdateCacheItem(request);

        return RedirectToAction(nameof(Apply), new ApplicationRequest
        {
            EncodedAccountId = request.EncodedAccountId,
            EncodedPledgeId = request.EncodedPledgeId,
            CacheKey = request.CacheKey
        });
    }

    [HideAccountNavigation(false)]
    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpGet]
    [Route("accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply/contact-details")]
    public async Task<IActionResult> ContactDetails(ContactDetailsRequest contactDetailsRequest)
    {
        var viewModel = await opportunitiesOrchestrator.GetContactDetailsViewModel(contactDetailsRequest);

        return View(viewModel);
    }

    [HideAccountNavigation(false)]
    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply/contact-details")]
    public async Task<IActionResult> ContactDetails(ContactDetailsPostRequest contactDetailsPostRequest)
    {
        await opportunitiesOrchestrator.UpdateCacheItem(contactDetailsPostRequest);

        return RedirectToAction(nameof(Apply), new
        {
            encodedAccountId = contactDetailsPostRequest.EncodedAccountId,
            encodedPledgeId = contactDetailsPostRequest.EncodedPledgeId,
            cacheKey = contactDetailsPostRequest.CacheKey
        });
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpGet]
    [Route("/accounts/{encodedAccountId}/opportunities/{encodedPledgeId}/apply/application-details/funding-estimate")]
    public async Task<IActionResult> GetFundingEstimate(GetFundingEstimateRequest request)
    {
        var result = await opportunitiesOrchestrator.GetFundingEstimate(request);

        return Json(result);
    }
}