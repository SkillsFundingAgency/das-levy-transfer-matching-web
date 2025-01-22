using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators;

public class ApplicationsOrchestrator : OpportunitiesOrchestratorBase, IApplicationsOrchestrator
{
    private readonly IApplicationsService _applicationsService;
    private readonly IEncodingService _encodingService;
    private readonly Infrastructure.Configuration.FeatureToggles _featureToggles;
    private readonly IUserService _userService;

    public ApplicationsOrchestrator(IApplicationsService applicationsService, IEncodingService encodingService, 
        Infrastructure.Configuration.FeatureToggles featureToggles, IUserService userService)
    {
        _applicationsService = applicationsService;
        _encodingService = encodingService;
        _featureToggles = featureToggles;
        _userService = userService;
    }

    public async Task<GetApplicationsViewModel> GetApplications(GetApplicationsRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _applicationsService.GetApplications(request.AccountId, request.Page.Value, GetApplicationsRequest.PageSize, cancellationToken);

        var applicationViewModels = result.Items?.Select(app => new GetApplicationsViewModel.ApplicationViewModel
        {
            EncodedApplicationId = _encodingService.Encode(app.Id, EncodingType.PledgeApplicationId),
            DasAccountName = app.IsNamePublic ? app.DasAccountName : "Opportunity",
            CreatedOn = app.CreatedOn,
            Status = app.Status,
            NumberOfApprentices = app.NumberOfApprentices,
            PledgeReference = _encodingService.Encode(app.PledgeId, EncodingType.PledgeId),
            IsNamePublic = app.IsNamePublic,
            EstimatedTotalCost = app.TotalAmount.ToCurrencyString()
        }).ToList();

        var viewModel = new GetApplicationsViewModel
        {
            Paging = GetPagingData(result),
            Applications = applicationViewModels,
            EncodedAccountId = request.EncodedAccountId,
            RenderViewApplicationDetailsHyperlink = _featureToggles.FeatureToggleCanViewApplicationDetails
        };

        return viewModel;
    }

    private PagingData GetPagingData<T>(PagedResponse<T> response)
    {
        return new PagingData()
        {
            Page = response.Page,
            PageSize = response.PageSize,
            TotalPages = response.TotalPages,
            TotalItems = response.TotalItems,
            ShowPageLinks = response.Page != 1 || response.TotalItems > response.PageSize,
            PageLinks = BuildPageLinks(response),
            PageStartRow = (response.Page - 1) * response.PageSize + 1,
            PageEndRow = response.Page * response.PageSize > response.TotalItems ? response.TotalItems : response.Page * response.PageSize,
        };
    }

    public IEnumerable<PageLink> BuildPageLinks<T>(PagedResponse<T> pledgesResponse)
    {
        var links = new List<PageLink>();
        var totalPages = (int)Math.Ceiling((double)pledgesResponse.TotalItems / pledgesResponse.PageSize);
        var totalPageLinks = totalPages < 5 ? totalPages : 5;

        //previous link
        if (totalPages > 1 && pledgesResponse.Page > 1)
        {
            links.Add(new PageLink
            {
                Label = "Previous",
                AriaLabel = "Previous page",
                RouteData = BuildRouteData(pledgesResponse.Page - 1)
            });
        }

        //numbered links
        var pageNumberSeed = 1;
        if (totalPages > 5 && pledgesResponse.Page > 3)
        {
            pageNumberSeed = pledgesResponse.Page - 2;

            if (pledgesResponse.Page > totalPages - 2)
                pageNumberSeed = totalPages - 4;
        }

        for (var i = 0; i < totalPageLinks; i++)
        {
            var link = new PageLink
            {
                Label = (pageNumberSeed + i).ToString(),
                AriaLabel = $"Page {pageNumberSeed + i}",
                IsCurrent = pageNumberSeed + i == pledgesResponse.Page ? true : (bool?)null,
                RouteData = BuildRouteData(pageNumberSeed + i)
            };
            links.Add(link);
        }

        //next link
        if (totalPages > 1 && pledgesResponse.Page < totalPages)
        {
            links.Add(new PageLink
            {
                Label = "Next",
                AriaLabel = "Next page",
                RouteData = BuildRouteData(pledgesResponse.Page + 1)
            });
        }

        return links;
    }

    private static Dictionary<string, string> BuildRouteData(int pageNumber)
    {
        return new Dictionary<string, string> { { "page", pageNumber.ToString() } };
    }

    public async Task<ApplicationViewModel> GetApplication(ApplicationRequest request)
    {
        var result = await _applicationsService.GetApplication(request.AccountId, request.ApplicationId);

        if (result == null)
        {
            return null;
        }

        var isOwnerOrTransactor = _userService.IsOwnerOrTransactor(request.EncodedAccountId);
        var encodedOpportunityId = _encodingService.Encode(result.OpportunityId, EncodingType.PledgeId);
        var encodedSenderPublicAccountId = _encodingService.Encode(result.SenderEmployerAccountId, EncodingType.PublicAccountId);

        var opportunitySummaryViewModelOptions = new GetOpportunitySummaryViewModelOptions
        {
            Sectors = result.Sectors,
            JobRoles = result.JobRoles,
            Levels = result.Levels,
            Locations = result.PledgeLocations,
            AllSectors = result.AllSectors,
            AllJobRoles = result.AllJobRoles,
            AllLevels = result.AllLevels,
            Amount = result.RemainingAmount,
            IsNamePublic = result.IsNamePublic,
            DasAccountName = result.PledgeEmployerAccountName,
            EncodedPledgeId = encodedOpportunityId,
            HideFooter = true
        };

        return new ApplicationViewModel
        {
            OpportunitySummaryViewModel = GetOpportunitySummaryViewModel(opportunitySummaryViewModelOptions),
            PledgeEmployerAccountName = result.PledgeEmployerAccountName,
            EncodedAccountId = request.EncodedAccountId,
            EncodedApplicationId = request.EncodedApplicationId,
            IsNamePublic = result.IsNamePublic,
            JobRole = result.StandardTitle,
            Level = result.StandardLevel,
            Locations = result.PledgeLocations,
            NumberOfApprentices = result.NumberOfApprentices,
            StartBy = result.StartBy,
            Status = result.Status,
            EncodedOpportunityId = encodedOpportunityId,
            EstimatedTotalCost = result.TotalAmount.ToCurrencyString(),
            CanAcceptFunding = isOwnerOrTransactor && result.Status == ApplicationStatus.Approved,
            CanUseTransferFunds = isOwnerOrTransactor && result.Status == ApplicationStatus.Accepted,
            EncodedSenderPublicAccountId = encodedSenderPublicAccountId,
            RenderCanUseTransferFundsStartButton = _featureToggles.FeatureToggleRenderCanUseTransferFundsStartButton,
            DisplayCurrentFundsBalance = result.AmountUsed > 0 || result.NumberOfApprenticesUsed > 0,
            AmountUsed = result.AmountUsed.ToCurrencyString(),
            AmountRemaining = (result.TotalAmount - result.AmountUsed) < 0 ? 0.ToCurrencyString() : (result.TotalAmount - result.AmountUsed).ToCurrencyString(),
            NumberOfApprenticesRemaining = (result.NumberOfApprentices - result.NumberOfApprenticesUsed) < 0 ? 0 : (result.NumberOfApprentices - result.NumberOfApprenticesUsed),
            CanWithdraw = isOwnerOrTransactor && result.Status == ApplicationStatus.Pending,
            RenderWithdrawAfterAcceptanceButton = isOwnerOrTransactor && result.Status == ApplicationStatus.Accepted && result.IsWithdrawableAfterAcceptance
        };
    }

    public async Task SetApplicationAcceptance(ApplicationPostRequest request)
    {
        await _applicationsService.SetApplicationAcceptance(new SetApplicationAcceptanceRequest
        {
            ApplicationId = request.ApplicationId,
            AccountId = request.AccountId,
            UserDisplayName = _userService.GetUserDisplayName(),
            UserId = _userService.GetUserId(),
            Acceptance = (SetApplicationAcceptanceRequest.ApplicationAcceptance)request.SelectedAction
        });
    }

    public async Task<AcceptedViewModel> GetAcceptedViewModel(AcceptedRequest request)
    {
        var result = await _applicationsService.GetAccepted(request.AccountId, request.ApplicationId);

        if (result == null)
        {
            return null;
        }

        var encodedPledgeId = _encodingService.Encode(result.OpportunityId, EncodingType.PledgeId);

        return new AcceptedViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            EncodedApplicationId = request.EncodedApplicationId,
            EmployerNameAndReference = $"{result.EmployerAccountName} ({encodedPledgeId})",
        };
    }

    public async Task<DeclinedViewModel> GetDeclinedViewModel(DeclinedRequest request)
    {
        var result = await _applicationsService.GetDeclined(request.AccountId, request.ApplicationId);

        if (result == null)
        {
            return null;
        }

        var encodedPledgeId = _encodingService.Encode(result.OpportunityId, EncodingType.PledgeId);

        return new DeclinedViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            EncodedApplicationId = request.EncodedApplicationId,
            EmployerNameAndReference = $"{result.EmployerAccountName} ({encodedPledgeId})",
        };
    }

    public async Task<WithdrawnViewModel> GetWithdrawnViewModel(WithdrawnRequest request)
    {
        var result = await _applicationsService.GetWithdrawn(request.AccountId, request.ApplicationId);

        if (result == null)
        {
            return null;
        }

        var encodedPledgeId = _encodingService.Encode(result.OpportunityId, EncodingType.PledgeId);

        return new WithdrawnViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            EncodedApplicationId = request.EncodedApplicationId,
            EmployerNameAndReference = $"{result.EmployerAccountName} ({encodedPledgeId})",
        };
    }

    public async Task<WithdrawalConfirmationViewModel> GetWithdrawalConfirmationViewModel(WithdrawalConfirmationRequest request)
    {
        var result = await _applicationsService.GetWithdrawalConfirmation(request.AccountId, request.ApplicationId);

        return new WithdrawalConfirmationViewModel
        {
            PledgeEmployerName = result.PledgeEmployerName,
            EncodedAccountId = _encodingService.Encode(request.AccountId, EncodingType.AccountId),
            EncodedApplicationId = _encodingService.Encode(request.ApplicationId, EncodingType.PledgeApplicationId),
            EncodedPledgeId = _encodingService.Encode(result.PledgeId, EncodingType.PledgeId)
        };
    }

    public async Task WithdrawApplicationAfterAcceptance(ConfirmWithdrawalPostRequest request)
    {
        var withDrawApplicationAfterAcceptanceRequest = new WithdrawApplicationAfterAcceptanceRequest
        {
            UserId = _userService.GetUserId(),
            UserDisplayName = _userService.GetUserDisplayName()
        };

        await _applicationsService.WithdrawApplicationAfterAcceptance(withDrawApplicationAfterAcceptanceRequest, request.AccountId, request.ApplicationId);
    }
}