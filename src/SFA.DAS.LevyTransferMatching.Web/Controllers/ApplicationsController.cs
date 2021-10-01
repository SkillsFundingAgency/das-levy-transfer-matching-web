﻿using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using ApplicationRequest = SFA.DAS.LevyTransferMatching.Web.Models.Applications.ApplicationRequest;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [Authorize(Policy = PolicyNames.ManageAccount)]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationsOrchestrator _applicationsOrchestrator;

        public ApplicationsController(IApplicationsOrchestrator applicationsOrchestrator)
        {
            _applicationsOrchestrator = applicationsOrchestrator;
        }

        [HttpGet]
        [Route("/accounts/{encodedAccountId}/applications")]
        public async Task<IActionResult> Applications(GetApplicationsRequest request)
        {
            var viewModel = await _applicationsOrchestrator.GetApplications(request);
            
            return View(viewModel);
        }

        [HttpGet]
        [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}")]
        public async Task<IActionResult> Application(ApplicationRequest request)
        {
            var viewModel = await _applicationsOrchestrator.GetApplication(request);

            if (viewModel != null)
            {
                return View(viewModel);
            }

            return NotFound();
        }

        [HttpPost]
        [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}")]
        public async Task<IActionResult> ApplicationStatus([FromServices] AbstractValidator<ApplicationStatusPostRequest> validator, ApplicationStatusPostRequest request, CancellationToken cancellationToken = default)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState, string.Empty);

                return RedirectToAction("ApplicationStatus");
            }

            await _applicationsOrchestrator.AcceptFunding(new AcceptFundingPostRequest{
                ApplicationId = request.ApplicationId,
                AccountId = request.AccountId,
            }, cancellationToken);

            return RedirectToAction("Applications");
        }

    }
}
