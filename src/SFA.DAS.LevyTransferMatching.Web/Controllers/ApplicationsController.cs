using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

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
        public async Task<IActionResult> ApplicationStatus(ApplicationStatusRequest applicationStatusRequest)
        {
            var viewModel = await _applicationsOrchestrator.GetApplicationStatusViewModel(applicationStatusRequest);

            if (viewModel != null)
            {
                return View(viewModel);
            }

            return NotFound();
        }
    }
}
