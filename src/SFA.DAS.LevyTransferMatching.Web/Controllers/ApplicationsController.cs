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

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = PolicyNames.ManageAccount)]
        [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}")]
        public async Task<IActionResult> Application(ApplicationPostRequest request)
        {
            if (request.SelectedAction == ApplicationViewModel.ApprovalAction.None)
            {
                return RedirectToAction("Applications", new { EncodedAccountId = request.EncodedAccountId });
            }

            await _applicationsOrchestrator.SetApplicationAcceptance(request);

            switch (request.SelectedAction)
            {
                case ApplicationViewModel.ApprovalAction.Accept:
                    return Redirect($"/accounts/{request.EncodedAccountId}/applications/{request.EncodedApplicationId}/accepted");
                case ApplicationViewModel.ApprovalAction.Withdraw:
                    return Redirect($"/accounts/{request.EncodedAccountId}/applications/{request.EncodedApplicationId}/withdrawn");
                default:
                    return Redirect($"/accounts/{request.EncodedAccountId}/applications/{request.EncodedApplicationId}/declined");
            }
        }

        [HttpGet]
        [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}/accepted")]
        public async Task<IActionResult> Accepted(AcceptedRequest request)
        {
            var viewModel = await _applicationsOrchestrator.GetAcceptedViewModel(request);

            return View(viewModel);
        }

        [HttpGet]
        [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}/declined")]
        public async Task<IActionResult> Declined(DeclinedRequest request)
        {
            var viewModel = await _applicationsOrchestrator.GetDeclinedViewModel(request);

            return View(viewModel);
        }

        [HttpGet]
        [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}/withdrawn")]
        public async Task<IActionResult> Withdrawn(WithdrawnRequest request)
        {
            var viewModel = await _applicationsOrchestrator.GetWithdrawnViewModel(request);

            return View(viewModel);
        }
    }
}
