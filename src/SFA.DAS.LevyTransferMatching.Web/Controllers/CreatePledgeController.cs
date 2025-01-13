using FluentValidation.AspNetCore;
using SFA.DAS.LevyTransferMatching.Web.Authentication;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Orchestrators;
using SFA.DAS.LevyTransferMatching.Web.ValidatorInterceptors;

namespace SFA.DAS.LevyTransferMatching.Web.Controllers;

[Authorize(Policy = PolicyNames.ViewAccount)]
[Route("accounts/{encodedAccountId}/pledges/create")]
public class CreatePledgeController(ICreatePledgeOrchestrator orchestrator) : Controller
{
    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("inform")]
    public IActionResult Inform(string encodedAccountId)
    {
        var viewModel = orchestrator.GetInformViewModel(encodedAccountId);
        return View(viewModel);
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("")]
    public async Task<IActionResult> Create(CreateRequest request)
    {
        var viewModel = await orchestrator.GetCreateViewModel(request);
        return View(viewModel);
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Create(CreatePostRequest request)
    {
        var pledge = await orchestrator.CreatePledge(request);
        return RedirectToAction("Confirmation", new ConfirmationRequest { EncodedAccountId = request.EncodedAccountId, EncodedPledgeId = pledge });
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("amount")]
    public async Task<IActionResult> Amount(AmountRequest request)
    {
        var viewModel = await orchestrator.GetAmountViewModel(request);
        return View(viewModel);
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("amount")]
    public async Task<IActionResult> Amount(AmountPostRequest request)
    {
        await orchestrator.UpdateCacheItem(request);
        return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("organisation")]
    public async Task<IActionResult> Organisation(OrganisationNameRequest request)
    {
        var viewModel = await orchestrator.GetOrganisationNameViewModel(request);
        return View(viewModel);
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("organisation")]
    public async Task<IActionResult> Organisation(OrganisationNamePostRequest request)
    {
        await orchestrator.UpdateCacheItem(request);
        return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("sector")]
    public async Task<IActionResult> Sector(SectorRequest request)
    {
        var viewModel = await orchestrator.GetSectorViewModel(request);
        return View(viewModel);
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("sector")]
    public async Task<IActionResult> Sector(SectorPostRequest request)
    {
        await orchestrator.UpdateCacheItem(request);
        return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("job-role")]
    public async Task<IActionResult> JobRole(JobRoleRequest request)
    {
        var viewModel = await orchestrator.GetJobRoleViewModel(request);
        return View(viewModel);
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("job-role")]
    public async Task<IActionResult> JobRole(JobRolePostRequest request)
    {
        await orchestrator.UpdateCacheItem(request);
        return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("level")]
    public async Task<IActionResult> Level(LevelRequest request)
    {
        var viewModel = await orchestrator.GetLevelViewModel(request);
        return View(viewModel);
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("level")]
    public async Task<IActionResult> Level(LevelPostRequest request)
    {
        await orchestrator.UpdateCacheItem(request);
        return RedirectToAction("Create", new CreateRequest { EncodedAccountId = request.EncodedAccountId, CacheKey = request.CacheKey });
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("location")]
    public async Task<IActionResult> Location(LocationRequest request)
    {
        var viewModel = await orchestrator.GetLocationViewModel(request);

        return View(viewModel);
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("location")]
    public async Task<IActionResult> Location(LocationPostRequest request)
    {
        var multipleValidLocations = new Dictionary<int, IEnumerable<string>>();

        var errors = await orchestrator.ValidateLocations(request, multipleValidLocations);

        if (errors.Count != 0)
        {
            AddLocationErrorsToModelState(errors);

            return RedirectToAction(nameof(Location), new { request.EncodedAccountId, request.AccountId, request.CacheKey });
        }

        await orchestrator.UpdateCacheItem(request);

        if (multipleValidLocations.Count != 0 && !request.AllLocationsSelected)
        {
            // Then surface a view to allow them to select the correct
            // location, from a set of multiple valid locations
            return RedirectToAction(nameof(LocationSelect), new { request.EncodedAccountId, request.CacheKey });
        }

        return RedirectToAction("Create", new CreateRequest { EncodedAccountId = request.EncodedAccountId, CacheKey = request.CacheKey });
    }

    [Authorize(Policy=PolicyNames.IsAuthenticated)]
    [Route("location/select")]
    public async Task<IActionResult> LocationSelect(LocationSelectRequest request)
    {
        var viewModel = await orchestrator.GetLocationSelectViewModel(request);

        return View(viewModel);
    }

    [Authorize(Policy=PolicyNames.IsAuthenticated)]
    [Route("location/select")]
    [HttpPost]
    public async Task<IActionResult> LocationSelect([CustomizeValidator(Interceptor = typeof(LocationSelectPostRequestValidatorInterceptor))] LocationSelectPostRequest request)
    {
        await orchestrator.UpdateCacheItem(request);

        return RedirectToAction("Create", new CreateRequest { EncodedAccountId = request.EncodedAccountId, CacheKey = request.CacheKey });
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpGet]
    [Route("confirmation")]
    public IActionResult Confirmation(ConfirmationRequest request)
    {
        return View(new ConfirmationViewModel { EncodedAccountId = request.EncodedAccountId, EncodedPledgeId = request.EncodedPledgeId });
    }

    private void AddLocationErrorsToModelState(Dictionary<int, string> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError($"Locations[{error.Key}]", error.Value);
        }
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [Route("auto-approval")]
    public async Task<IActionResult> AutoApproval(AutoApproveRequest request)
    {
        var viewModel = await orchestrator.GetAutoApproveViewModel(request);
        return View(viewModel);
    }

    [Authorize(Policy = PolicyNames.ManageAccount)]
    [HttpPost]
    [Route("auto-approval")]
    public async Task<IActionResult> AutoApproval(AutoApprovePostRequest request)
    {
        await orchestrator.UpdateCacheItem(request);
        return RedirectToAction("Create", new { request.EncodedAccountId, request.CacheKey });
    }
}