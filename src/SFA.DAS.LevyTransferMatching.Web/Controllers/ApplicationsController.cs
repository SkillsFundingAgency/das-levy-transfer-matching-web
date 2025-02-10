using SFA.DAS.LevyTransferMatching.Web.Attributes;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers;

[Authorize(Policy = PolicyNames.ViewAccount)]
public class ApplicationsController(IApplicationsOrchestrator applicationsOrchestrator) : Controller
{
    [HttpGet]
    [HideAccountNavigation(false)]
    [Route("/accounts/{encodedAccountId}/applications", Name = RouteNames.Applications)]
    public async Task<IActionResult> Applications(GetApplicationsRequest request)
    {
        var viewModel = await applicationsOrchestrator.GetApplications(request);
            
        return View(viewModel);
    }

    [HttpGet]
    [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}")]
    public async Task<IActionResult> Application(ApplicationRequest request)
    {
        var viewModel = await applicationsOrchestrator.GetApplication(request);

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
        if (request.SelectedAction == ApplicationViewModel.ApprovalAction.None)
        {
            return RedirectToAction("Applications", new { request.EncodedAccountId });
        }

        await applicationsOrchestrator.SetApplicationAcceptance(request);

        if (request.SelectedAction == ApplicationViewModel.ApprovalAction.Accept)
        {
            return Redirect($"/accounts/{request.EncodedAccountId}/applications/{request.EncodedApplicationId}/accepted");
        }

        if (request.SelectedAction == ApplicationViewModel.ApprovalAction.Withdraw)
        {
            return Redirect($"/accounts/{request.EncodedAccountId}/applications/{request.EncodedApplicationId}/withdrawn");
        }
        
        return Redirect($"/accounts/{request.EncodedAccountId}/applications/{request.EncodedApplicationId}/declined");
    }

    [HttpGet]
    [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}/accepted")]
    public async Task<IActionResult> Accepted(AcceptedRequest request)
    {
        var viewModel = await applicationsOrchestrator.GetAcceptedViewModel(request);

        if (viewModel != null)
        {
            return View(viewModel);
        }

        return NotFound();
    }

    [HttpGet]
    [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}/declined")]
    public async Task<IActionResult> Declined(DeclinedRequest request)
    {
        var viewModel = await applicationsOrchestrator.GetDeclinedViewModel(request);

        if (viewModel != null)
        {
            return View(viewModel);
        }

        return NotFound();
    }

    [HttpGet]
    [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}/withdrawn")]
    public async Task<IActionResult> Withdrawn(WithdrawnRequest request)
    {
        var viewModel = await applicationsOrchestrator.GetWithdrawnViewModel(request);

        if (viewModel != null)
        {
            return View(viewModel);
        }

        return NotFound();
    }

    [HttpGet]
    [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}/withdrawal-confirmation")]
    public async Task<IActionResult> WithdrawalConfirmation(WithdrawalConfirmationRequest request)
    {
        var viewModel = await applicationsOrchestrator.GetWithdrawalConfirmationViewModel(request);

        return View(viewModel);
    }

    [HttpPost]
    [Route("/accounts/{encodedAccountId}/applications/{encodedApplicationId}/withdrawal-confirmation")]
    public async Task<IActionResult> ConfirmWithdrawal(ConfirmWithdrawalPostRequest request)
    {
        if (request.HasConfirmed.Value)
        {
            await applicationsOrchestrator.WithdrawApplicationAfterAcceptance(request);

            return RedirectToAction("Withdrawn", new { request.EncodedAccountId, request.EncodedApplicationId });
        }

        return RedirectToAction("Applications", new { request.EncodedAccountId });
    }
}