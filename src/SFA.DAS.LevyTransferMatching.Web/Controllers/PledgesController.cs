using System.Threading;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers;

[Authorize(Policy = PolicyNames.ViewAccount)]
[Route("accounts/{encodedAccountId}/pledges")]
public class PledgesController(IPledgeOrchestrator orchestrator) : Controller
{
    [Route("", Name = "pledges")]
    public async Task<IActionResult> Pledges(PledgesRequest request)
    {
        var viewModel = await orchestrator.GetPledgesViewModel(request);
        return View(viewModel);
    }

    [HttpGet]
    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("{encodedPledgeId}/close")]
    public IActionResult Close(CloseRequest request)
    {
        var viewModel = orchestrator.GetCloseViewModel(request);
        return View(viewModel);
    }

    [HttpPost]
    [Route("{encodedPledgeId}/close")]
    public async Task<IActionResult> Close(ClosePostRequest closePostRequest)
    {
        if (closePostRequest.HasConfirmed.Value)
        {
            await orchestrator.ClosePledge(closePostRequest);

            TempData.AddFlashMessage("Transfer pledge closed", $"You closed the transfer pledge {closePostRequest.EncodedPledgeId}.", TempDataDictionaryExtensions.FlashMessageLevel.Success);
            return RedirectToAction(nameof(Pledges), new { closePostRequest.EncodedAccountId });
        }
        return RedirectToAction(nameof(Applications), new { closePostRequest.EncodedAccountId, closePostRequest.EncodedPledgeId });
    }

    [Route("{EncodedPledgeId}/detail")]
    public IActionResult Detail(DetailRequest request)
    {
        var viewModel = orchestrator.GetDetailViewModel(request);
        return View(viewModel);
    }

    [HttpGet]
    [Route("{encodedPledgeId}/applications")]
    public async Task<IActionResult> Applications(ApplicationsRequest request)
    {
        var response = await orchestrator.GetApplications(request);
        return View(response);
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("{encodedPledgeId}/applications")]
    public IActionResult Applications(ApplicationsPostRequest request)
    {
        return RedirectToAction(nameof(RejectApplications), new { request.EncodedAccountId, request.EncodedPledgeId, request.ApplicationsToReject });
    }
        
    [Authorize(Policy=PolicyNames.IsAuthenticated)]
    [HttpGet]
    [Route("{encodedPledgeId}/applications/reject-applications")]
    public async Task<IActionResult> RejectApplications(RejectApplicationsRequest request)
    {
        var rejectApplicationsViewModel = await orchestrator.GetRejectApplicationsViewModel(request);
        return View(rejectApplicationsViewModel);
    }

    [Authorize(Policy=PolicyNames.IsAuthenticated)]
    [Route("{encodedPledgeId}/applications/reject-applications")]
    [HttpPost]
    public async Task<IActionResult> RejectApplications(RejectApplicationsPostRequest request)
    {
        if (request.RejectConfirm.Value)
        {
            await orchestrator.RejectApplications(request);
            SetRejectedApplicationsBanner(request.ApplicationsToReject.Count);
        }
        return RedirectToAction(nameof(Applications), new { request.EncodedAccountId, request.EncodedPledgeId });
    }
        
    private void SetRejectedApplicationsBanner(int rejectedApplications)
    {
        var bannerMessage = rejectedApplications > 1 ? $"{rejectedApplications} applications have been rejected" :
            $"{rejectedApplications} application has been rejected";

        TempData.AddFlashMessage(bannerMessage, string.Empty, TempDataDictionaryExtensions.FlashMessageLevel.Success);
    }

    [HttpGet]
    [Route("{encodedPledgeId}/applications/download")]
    public async Task<IActionResult> DownloadApplicationsCsv(ApplicationsRequest request)
    {
        var response = await orchestrator.GetPledgeApplicationsDownloadModel(request);

        return new FileContentResult(response, "text/csv");
    }

    [HttpGet]
    [Route("{encodedPledgeId}/applications/{encodedApplicationId}")]
    public async Task<IActionResult> Application(ApplicationRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await orchestrator.GetApplicationViewModel(request, cancellationToken);

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

        await orchestrator.SetApplicationOutcome(request);

        if (request.SelectedAction == ApplicationPostRequest.ApprovalAction.Approve)
        {
            return RedirectToAction("ApplicationApproved", new { request.EncodedAccountId, request.EncodedPledgeId, request.EncodedApplicationId });
        }

        TempData.AddFlashMessage("Application rejected", $"You rejected the {request.EmployerAccountName} application.", TempDataDictionaryExtensions.FlashMessageLevel.Success);
        return RedirectToAction("Applications", new { request.EncodedAccountId, request.EncodedPledgeId });
    }

    [HttpGet]
    [Route("{encodedPledgeId}/applications/{encodedApplicationId}/approved")]
    public async Task<IActionResult> ApplicationApproved(ApplicationApprovedRequest request)
    {
        var viewModel = await orchestrator.GetApplicationApprovedViewModel(request);

        return View(viewModel);
    }

    [HttpGet]
    [Route("{encodedPledgeId}/applications/{encodedApplicationId}/approval-options")]
    public async Task<IActionResult> ApplicationApprovalOptions(ApplicationApprovalOptionsRequest request)
    {
        var viewModel = await orchestrator.GetApplicationApprovalOptionsViewModel(request);

        if (viewModel.IsApplicationPending)
        {
            return View(viewModel);
        }

        return RedirectToAction("Application", new { request.EncodedAccountId, request.EncodedPledgeId, request.EncodedApplicationId });
    }

    [HttpPost]
    [Route("{encodedPledgeId}/applications/{encodedApplicationId}/approval-options")]
    public async Task<IActionResult> ApplicationApprovalOptions(ApplicationApprovalOptionsPostRequest request)
    {
        await orchestrator.SetApplicationApprovalOptions(request);
        return RedirectToAction("ApplicationApproved", new { request.EncodedAccountId, request.EncodedPledgeId, request.EncodedApplicationId });
    }
}