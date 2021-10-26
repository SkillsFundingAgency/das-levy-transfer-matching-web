using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LevyTransferMatching.Web.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers
{
    [Authorize(Policy = PolicyNames.ViewAccount)]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationsOrchestrator _applicationsOrchestrator;

        public ApplicationsController(IApplicationsOrchestrator applicationsOrchestrator)
        {
            _applicationsOrchestrator = applicationsOrchestrator;
        }

        [HttpGet]
        [HideAccountNavigation(false)]
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
        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}")]
        public async Task<IActionResult> Application(ApplicationPostRequest request)
        {
            await _applicationsOrchestrator.SetApplicationAcceptance(request);

            if (request.SelectedAction == ApplicationViewModel.ApprovalAction.Accept)

                return Redirect($"/accounts/{request.EncodedAccountId}/applications/{request.EncodedApplicationId}/accepted");

            // TODO: Implemnentation of decline journey
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}/accepted")]
        public async Task<IActionResult> Accepted(AcceptedRequest request)
        {
            var viewModel = await _applicationsOrchestrator.GetAcceptedViewModel(request);

            if (viewModel != null)
            {
                return View(viewModel);
            }

            return NotFound();
        }
    }
}
