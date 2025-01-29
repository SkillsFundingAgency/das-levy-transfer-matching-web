using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;
using ApplyRequest = SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types.ApplyRequest;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators;

public class OpportunitiesOrchestrator(
    IDateTimeService dateTimeService, 
    IOpportunitiesService opportunitiesService, 
    IUserService userService, 
    IEncodingService encodingService, 
    ICacheStorageService cacheStorageService) : OpportunitiesOrchestratorBase, IOpportunitiesOrchestrator
{
    private const int MaximumNumberAdditionalEmailAddresses = 4;

    public async Task<DetailViewModel> GetDetailViewModel(DetailRequest detailRequest)
    {
        var response = await opportunitiesService.GetDetail(detailRequest.PledgeId);

        if (response.Opportunity == null)
            return null;

        var encodedPledgeId = encodingService.Encode(response.Opportunity.Id, EncodingType.PledgeId);

        var opportunitySummaryViewModelOptions = new GetOpportunitySummaryViewModelOptions
        {
            Sectors = response.Opportunity.Sectors,
            JobRoles = response.Opportunity.JobRoles,
            Levels = response.Opportunity.Levels,
            Locations = response.Opportunity.Locations,
            AllSectors = response.Sectors,
            AllJobRoles = response.JobRoles,
            AllLevels = response.Levels,
            Amount = response.Opportunity.Amount,
            IsNamePublic = response.Opportunity.IsNamePublic,
            DasAccountName = response.Opportunity.DasAccountName,
            EncodedPledgeId = encodedPledgeId,
        };

        var opportunitySummaryViewModel = GetOpportunitySummaryViewModel(opportunitySummaryViewModelOptions);

        return new DetailViewModel
        {
            EmployerName = response.Opportunity.DasAccountName,
            EncodedPledgeId = encodedPledgeId,
            IsNamePublic = response.Opportunity.IsNamePublic,
            OpportunitySummaryView = opportunitySummaryViewModel,
            CommaSeparatedSectors = detailRequest.CommaSeparatedSectors,
            Page = detailRequest.Page,
            SortBy = detailRequest.SortBy
        };
    }

    public async Task<IndexViewModel> GetIndexViewModel(IndexRequest request)
    {
        var opportunitySort = OpportunitiesSortBy.ValueHighToLow;
        if (!string.IsNullOrEmpty(request.SortBy) 
            && Enum.TryParse<OpportunitiesSortBy>(request.SortBy, true, out var opportunitySortParsed))
        {
            opportunitySort = opportunitySortParsed;
        }
        var response = await opportunitiesService.GetIndex(request.Sectors, opportunitySort, request.Page ?? 1, IndexRequest.DefaultPageSize);

        return new IndexViewModel
        {
            SortBy = request.SortBy,
            CommaSeparatedSectors = request.CommaSeparatedSectors,
            Paging = GetPagingData(response, request),
            Opportunities = response?.Opportunities
                .Select(x => new IndexViewModel.Opportunity
                {
                    Amount = x.Amount,
                    EmployerName = x.IsNamePublic ? x.DasAccountName : "Opportunity",
                    ReferenceNumber = encodingService.Encode(x.Id, EncodingType.PledgeId),
                    Sectors = x.Sectors.ToReferenceDataDescriptionList(response.Sectors, "; "),
                    JobRoles = x.JobRoles.ToReferenceDataDescriptionList(response.JobRoles, "; "),
                    Levels = x.Levels.ToReferenceDataDescriptionList(response.Levels, descriptionSource: y => y.ShortDescription),
                    Locations = x.Locations.ToLocationsList()
                }).ToList(),
            Sectors = response?.Sectors,
            isSectorFilterApplied = request.Sectors != null && request.Sectors.Any()
        };
    }

    private IndexViewModel.PagingData GetPagingData(GetIndexResponse response, IndexRequest request)
    {
        return new IndexViewModel.PagingData()
        {
            Page = response.Page,
            PageSize = response.PageSize,
            TotalPages = response.TotalPages,
            TotalOpportunities = response.TotalOpportunities,
            ShowPageLinks = response.Page != 1 || response.TotalOpportunities > response.PageSize,
            PageLinks = BuildPageLinks(response, request),
            PageStartRow = (response.Page - 1) * response.PageSize + 1,
            PageEndRow = response.Page * response.PageSize > response.TotalOpportunities ? response.TotalOpportunities : response.Page * response.PageSize,
        };
    }

    public IEnumerable<IndexViewModel.PageLink> BuildPageLinks(GetIndexResponse response, IndexRequest request)
    {
        var links = new List<IndexViewModel.PageLink>();
        var totalPages = (int)Math.Ceiling((double)response.TotalOpportunities / response.PageSize);
        var totalPageLinks = totalPages < 7 ? totalPages : 7;

        //previous link
        if (totalPages > 1 && response.Page > 1)
        {
            links.Add(new IndexViewModel.PageLink
            {
                Label = "Previous",
                AriaLabel = "Previous page",
                RouteData = BuildRouteData(request, response.Page - 1)
            });
        }

        //numbered links
        var pageNumberSeed = 1;
        if (totalPages > 7 && response.Page > 3)
        {
            pageNumberSeed = response.Page - 2;

            if (response.Page > totalPages - 2)
                pageNumberSeed = totalPages - 4;
        }

        for (var i = 0; i < totalPageLinks; i++)
        {
            var link = new IndexViewModel.PageLink
            {
                Label = (pageNumberSeed + i).ToString(),
                AriaLabel = $"Page {pageNumberSeed + i}",
                IsCurrent = pageNumberSeed + i == response.Page ? true : (bool?)null,
                RouteData = BuildRouteData(request, pageNumberSeed + i)
            };
            links.Add(link);
        }

        //next link
        if (totalPages > 1 && response.Page < totalPages)
        {
            links.Add(new IndexViewModel.PageLink
            {
                Label = "Next",
                AriaLabel = "Next page",
                RouteData = BuildRouteData(request, response.Page + 1)
            });
        }

        return links;
    }

    private static Dictionary<string, string> BuildRouteData(IndexRequest request, int pageNumber)
    {
        var routeData = new Dictionary<string, string> { { "page", pageNumber.ToString() } };

        if (request.Sectors != null && request.Sectors.Any())
        {
            var allSectors = string.Join(",", request.Sectors.Select(Uri.EscapeDataString));
            routeData.Add("CommaSeparatedSectors", allSectors);
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            routeData.Add("SortBy", request.SortBy);
        }

        return routeData;
    }

    public async Task<SelectAccountViewModel> GetSelectAccountViewModel(SelectAccountRequest request)
    {
        var userId = userService.GetUserId();

        var ownerTransactorAccounts = userService.GetUserOwnerTransactorAccountIds();

        // Get the full detail of accounts, that the user has access to
        var result = await opportunitiesService.GetSelectAccount(request.OpportunityId, userId);

        var filteredAccounts = result.Accounts
            .Where((x) => ownerTransactorAccounts.Contains(x.EncodedAccountId));

        return new SelectAccountViewModel
        {
            Accounts = filteredAccounts
                .Select(x => new SelectAccountViewModel.Account
                {
                    EncodedAccountId = x.EncodedAccountId,
                    Name = x.Name,
                }),
            EncodedOpportunityId = request.EncodedOpportunityId,
        };
    }

    public async Task<ConfirmationViewModel> GetConfirmationViewModel(ConfirmationRequest request)
    {
        var result = await opportunitiesService.GetConfirmation(request.AccountId, request.PledgeId);
        return new ConfirmationViewModel
        {
            AccountName = result.AccountName,
            IsNamePublic = result.IsNamePublic,
            Reference = encodingService.Encode(request.PledgeId, EncodingType.PledgeId),
            EncodedAccountId = request.EncodedAccountId
        };
    }

    public async Task SubmitApplication(ApplyPostRequest request)
    {
        var cacheItem = await RetrieveCacheItem(request.CacheKey);

        var applyRequest = new ApplyRequest
        {
            EncodedAccountId = request.EncodedAccountId,
            Details = cacheItem.Details ?? string.Empty,
            StandardId = cacheItem.StandardId,
            NumberOfApprentices = cacheItem.NumberOfApprentices.Value,
            StartDate = cacheItem.StartDate.Value,
            HasTrainingProvider = cacheItem.HasTrainingProvider.Value,
            Sectors = cacheItem.Sectors,
            Locations = cacheItem.Locations,
            AdditionalLocation = cacheItem.AdditionalLocation ? cacheItem.AdditionLocationText : string.Empty,
            SpecificLocation = cacheItem.SpecificLocation ?? string.Empty,
            FirstName = cacheItem.FirstName ?? string.Empty,
            LastName = cacheItem.LastName ?? string.Empty,
            EmailAddresses = cacheItem.EmailAddresses,
            BusinessWebsite = cacheItem.BusinessWebsite ?? string.Empty,
            UserId = userService.GetUserId(),
            UserDisplayName = userService.GetUserDisplayName()
        };

        await opportunitiesService.PostApplication(request.AccountId, request.PledgeId, applyRequest);

        await cacheStorageService.DeleteFromCache(request.CacheKey.ToString());
    }

    public async Task<ApplyViewModel> GetApplyViewModel(ApplicationRequest request)
    {
        var applicationTask = RetrieveCacheItem(request.CacheKey);
        var applyResponseTask = opportunitiesService.GetApply(request.AccountId, request.PledgeId);

        await Task.WhenAll(applicationTask, applyResponseTask);

        var contactName = $"{applicationTask.Result.FirstName} {applicationTask.Result.LastName}";

        var opportunitySummaryViewModelOptions = new GetOpportunitySummaryViewModelOptions
        {
            Sectors = applyResponseTask.Result.Opportunity.Sectors,
            JobRoles = applyResponseTask.Result.Opportunity.JobRoles,
            Levels = applyResponseTask.Result.Opportunity.Levels,
            Locations = applyResponseTask.Result.Opportunity.Locations,
            AllSectors = applyResponseTask.Result.Sectors,
            AllJobRoles = applyResponseTask.Result.JobRoles,
            AllLevels = applyResponseTask.Result.Levels,
            Amount = applyResponseTask.Result.Opportunity.Amount,
            IsNamePublic = applyResponseTask.Result.Opportunity.IsNamePublic,
            DasAccountName = applyResponseTask.Result.Opportunity.DasAccountName,
            EncodedPledgeId = request.EncodedPledgeId,
        };

        var result = new ApplyViewModel
        {
            CacheKey = applicationTask.Result.Key,
            EncodedPledgeId = request.EncodedPledgeId,
            EncodedAccountId = request.EncodedAccountId,
            OpportunitySummaryViewModel = GetOpportunitySummaryViewModel(opportunitySummaryViewModelOptions),
            JobRole = applicationTask.Result.JobRole ?? "-",
            NumberOfApprentices = applicationTask.Result.NumberOfApprentices.HasValue ? applicationTask.Result.NumberOfApprentices.Value.ToString() : "-",
            StartBy = applicationTask.Result.StartDate.HasValue ? applicationTask.Result.StartDate.Value.ToShortDisplayString() : "-",
            HaveTrainingProvider = applicationTask.Result.HasTrainingProvider.ToApplyViewString(),
            Sectors = applicationTask.Result.Sectors?.ToList(),
            SectorOptions = applyResponseTask.Result.Sectors?.ToList(),
            MoreDetail = applicationTask.Result.Details ?? "-",
            ContactName = string.IsNullOrWhiteSpace(contactName) ? "-" : contactName,
            EmailAddresses = applicationTask.Result.EmailAddresses,
            WebsiteUrl = string.IsNullOrEmpty(applicationTask.Result.BusinessWebsite) ? "-" : applicationTask.Result.BusinessWebsite,
            AccessToMultipleAccounts = request.AccessToMultipleAccounts
        };

        if (applyResponseTask.Result.PledgeLocations.Any())
        {
            var locations = new List<string>();

            if (applicationTask.Result.Locations != null)
            {
                locations.AddRange(applyResponseTask.Result.PledgeLocations
                    .Where(x => applicationTask.Result.Locations.Contains(x.Id)).Select(y => y.Name).ToList());
            }

            if (applicationTask.Result.AdditionalLocation)
            {
                locations.Add(applicationTask.Result.AdditionLocationText);
            }

            result.Locations = locations.OrderBy(x => x);
        }
        else
        {
            result.Locations = new List<string> { applicationTask.Result.SpecificLocation };
        }

        return result;
    }

    public async Task<ContactDetailsViewModel> GetContactDetailsViewModel(ContactDetailsRequest contactDetailsRequest)
    {
        var getContactDetailsResult = await opportunitiesService.GetContactDetails(contactDetailsRequest.AccountId, contactDetailsRequest.PledgeId);

        if (getContactDetailsResult == null)
        {
            return null;
        }

        var opportunitySummaryViewModelOptions = new GetOpportunitySummaryViewModelOptions
        {
            Sectors = getContactDetailsResult.Sectors,
            JobRoles = getContactDetailsResult.JobRoles,
            Levels = getContactDetailsResult.Levels,
            Locations = getContactDetailsResult.Locations,
            AllSectors = getContactDetailsResult.AllSectors,
            AllJobRoles = getContactDetailsResult.AllJobRoles,
            AllLevels = getContactDetailsResult.AllLevels,
            Amount = getContactDetailsResult.Amount,
            IsNamePublic = getContactDetailsResult.IsNamePublic,
            DasAccountName = getContactDetailsResult.DasAccountName,
            EncodedPledgeId = contactDetailsRequest.EncodedPledgeId,
        };

        var opportunitySummaryViewModel = GetOpportunitySummaryViewModel(opportunitySummaryViewModelOptions);

        var cacheItem = await RetrieveCacheItem(contactDetailsRequest.CacheKey);

        var additionalEmailAddresses = cacheItem.EmailAddresses.Skip(1).ToList();

        var placeholders = Enumerable.Range(0, MaximumNumberAdditionalEmailAddresses - additionalEmailAddresses.Count)
            .Select(x => (string)null);

        additionalEmailAddresses.AddRange(placeholders);

        var viewModel = new ContactDetailsViewModel
        {
            EncodedAccountId = contactDetailsRequest.EncodedAccountId,
            EncodedPledgeId = contactDetailsRequest.EncodedPledgeId,
            CacheKey = contactDetailsRequest.CacheKey,
            FirstName = cacheItem.FirstName,
            LastName = cacheItem.LastName,
            EmailAddress = cacheItem.EmailAddresses.FirstOrDefault(),
            AdditionalEmailAddresses = additionalEmailAddresses.ToArray(),
            BusinessWebsite = cacheItem.BusinessWebsite,
            DasAccountName = getContactDetailsResult.DasAccountName,
            OpportunitySummaryViewModel = opportunitySummaryViewModel,
        };

        return viewModel;
    }

    public async Task UpdateCacheItem(ContactDetailsPostRequest contactDetailsPostRequest)
    {
        var cacheItem = await RetrieveCacheItem(contactDetailsPostRequest.CacheKey);

        cacheItem.FirstName = contactDetailsPostRequest.FirstName;
        cacheItem.LastName = contactDetailsPostRequest.LastName;

        cacheItem.EmailAddresses.Clear();
        cacheItem.EmailAddresses.Add(contactDetailsPostRequest.EmailAddress);
        cacheItem.EmailAddresses.AddRange(contactDetailsPostRequest.AdditionalEmailAddresses.Where(x => !string.IsNullOrWhiteSpace(x)));

        cacheItem.BusinessWebsite = contactDetailsPostRequest.BusinessWebsite;

        await cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
    }

    public async Task<MoreDetailsViewModel> GetMoreDetailsViewModel(MoreDetailsRequest request)
    {
        var applicationTask = RetrieveCacheItem(request.CacheKey);
        var moreDetailsResponseTask = opportunitiesService.GetMoreDetails(request.AccountId, request.PledgeId);

        await Task.WhenAll(applicationTask, moreDetailsResponseTask);

        var opportunitySummaryViewModelOptions = new GetOpportunitySummaryViewModelOptions
        {
            Sectors = moreDetailsResponseTask.Result.Opportunity.Sectors,
            JobRoles = moreDetailsResponseTask.Result.Opportunity.JobRoles,
            Levels = moreDetailsResponseTask.Result.Opportunity.Levels,
            Locations = moreDetailsResponseTask.Result.Opportunity.Locations,
            AllSectors = moreDetailsResponseTask.Result.Sectors,
            AllJobRoles = moreDetailsResponseTask.Result.JobRoles,
            AllLevels = moreDetailsResponseTask.Result.Levels,
            Amount = moreDetailsResponseTask.Result.Opportunity.Amount,
            IsNamePublic = moreDetailsResponseTask.Result.Opportunity.IsNamePublic,
            DasAccountName = moreDetailsResponseTask.Result.Opportunity.DasAccountName,
            EncodedPledgeId = request.EncodedPledgeId,
        };

        return new MoreDetailsViewModel
        {
            CacheKey = request.CacheKey,
            EncodedAccountId = request.EncodedAccountId,
            EncodedPledgeId = request.EncodedPledgeId,
            Details = applicationTask.Result.Details,
            OpportunitySummaryViewModel = GetOpportunitySummaryViewModel(opportunitySummaryViewModelOptions),
        };
    }

    public async Task<SectorViewModel> GetSectorViewModel(SectorRequest request)
    {
        var cacheItem = await RetrieveCacheItem(request.CacheKey);
        var response = await opportunitiesService.GetSector(request.AccountId, request.PledgeId);

        var opportunitySummaryViewModelOptions = new GetOpportunitySummaryViewModelOptions
        {
            Sectors = response.Opportunity.Sectors,
            JobRoles = response.Opportunity.JobRoles,
            Levels = response.Opportunity.Levels,
            Locations = response.Opportunity.Locations,
            AllSectors = response.Sectors,
            AllJobRoles = response.JobRoles,
            AllLevels = response.Levels,
            Amount = response.Opportunity.Amount,
            IsNamePublic = response.Opportunity.IsNamePublic,
            DasAccountName = response.Opportunity.DasAccountName,
            EncodedPledgeId = request.EncodedPledgeId,
        };

        return new SectorViewModel
        {
            CacheKey = request.CacheKey,
            EncodedAccountId = request.EncodedAccountId,
            EncodedPledgeId = request.EncodedPledgeId,
            Sectors = cacheItem.Sectors,
            SectorOptions = response.Sectors.ToList(),
            OpportunitySummaryViewModel = GetOpportunitySummaryViewModel(opportunitySummaryViewModelOptions),
            PledgeLocations = response.PledgeLocations.Select(x => new CheckboxListItem { Id = x.Id, Label = x.Name }).OrderBy(y => y.Label),
            HasPledgeLocations = response.PledgeLocations.Any(),
            Locations = cacheItem.Locations,
            AdditionalLocation = cacheItem.AdditionalLocation,
            AdditionalLocationText = cacheItem.AdditionLocationText,
            SpecificLocation = cacheItem.SpecificLocation
        };
    }

    public async Task UpdateCacheItem(MoreDetailsPostRequest request)
    {
        var cacheItem = await RetrieveCacheItem(request.CacheKey);

        cacheItem.Details = request.Details;

        await cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
    }

    public async Task UpdateCacheItem(ApplicationDetailsPostRequest request, int amount)
    {
        var cacheItem = await RetrieveCacheItem(request.CacheKey);

        cacheItem.JobRole = request.SelectedStandardTitle;
        cacheItem.StandardId = request.SelectedStandardId;
        cacheItem.NumberOfApprentices = request.ParsedNumberOfApprentices;
        cacheItem.StartDate = request.StartDate;
        cacheItem.HasTrainingProvider = request.HasTrainingProvider.Value;

        await cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
    }

    public async Task UpdateCacheItem(SectorPostRequest request)
    {
        var cacheItem = await RetrieveCacheItem(request.CacheKey);

        cacheItem.Sectors = request.Sectors;
        cacheItem.Locations = request.Locations;
        cacheItem.AdditionalLocation = request.AdditionalLocation;
        cacheItem.AdditionLocationText = request.AdditionalLocationText;
        cacheItem.SpecificLocation = request.SpecificLocation;

        await cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
    }

    private async Task<CreateApplicationCacheItem> RetrieveCacheItem(Guid key)
    {
        var result = await cacheStorageService.RetrieveFromCache<CreateApplicationCacheItem>(key.ToString());

        if (result == null)
        {
            result = new CreateApplicationCacheItem(key);
            await cacheStorageService.SaveToCache(key.ToString(), result, 1);
        }

        return result;
    }

    public async Task<ApplicationDetailsViewModel> GetApplicationViewModel(ApplicationDetailsRequest request)
    {
        var applicationDetailsTask = opportunitiesService.GetApplicationDetails(request.AccountId, request.PledgeId);
        var applicationTask = RetrieveCacheItem(request.CacheKey);

        await Task.WhenAll(applicationDetailsTask, applicationTask);

        var application = applicationTask.Result;
        var applicationDetails = applicationDetailsTask.Result;

        var opportunitySummaryViewModelOptions = new GetOpportunitySummaryViewModelOptions
        {
            Sectors = applicationDetails.Opportunity.Sectors,
            JobRoles = applicationDetails.Opportunity.JobRoles,
            Levels = applicationDetails.Opportunity.Levels,
            Locations = applicationDetails.Opportunity.Locations,
            AllSectors = applicationDetails.Sectors,
            AllJobRoles = applicationDetails.JobRoles,
            AllLevels = applicationDetails.Levels,
            Amount = applicationDetails.Opportunity.RemainingAmount,
            IsNamePublic = applicationDetails.Opportunity.IsNamePublic,
            DasAccountName = applicationDetails.Opportunity.DasAccountName,
            EncodedPledgeId = request.EncodedPledgeId,
        };

        return new ApplicationDetailsViewModel
        {
            CacheKey = request.CacheKey,
            EncodedAccountId = request.EncodedAccountId,
            EncodedPledgeId = request.EncodedPledgeId,
            JobRole = application.JobRole,
            NumberOfApprentices = application.NumberOfApprentices,
            Month = application.StartDate?.Month,
            Year = application.StartDate?.Year,
            HasTrainingProvider = application.HasTrainingProvider,
            OpportunitySummaryViewModel = GetOpportunitySummaryViewModel(opportunitySummaryViewModelOptions),
            MinYear = DateTime.Now.Year,
            MaxYear = DateTime.Now.FinancialYearEnd().Year,
            SelectStandardViewModel = new SelectStandardViewModel
            {
                Standards = applicationDetails.Standards.Select(app => new StandardsListItemViewModel
                {
                    Id = app.StandardUId,
                    LarsCode = app.LarsCode,
                    Level = app.Level,
                    Title = app.Title,
                    Selected = !string.IsNullOrEmpty(application.StandardId) && (app.StandardUId == application.StandardId) ? "selected" : null
                }),
            },
            CurrentFinancialYear = dateTimeService.UtcNow.ToTaxYearDescription()
        };
    }

    public async Task<ApplicationRequest> PostApplicationViewModel(ApplicationDetailsPostRequest request)
    {
        var applicationDetails = await opportunitiesService.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId);

        var amount = (await GetFundingEstimate(new GetFundingEstimateRequest
        {
            StartDate = request.StartDate.Value,
            SelectedStandardId = request.SelectedStandardId,
            NumberOfApprentices = request.ParsedNumberOfApprentices.Value,
            PledgeId = request.PledgeId
        }, applicationDetails)).Amount;

        request.SelectedStandardTitle = applicationDetails.Standards
            .FirstOrDefault(standard => standard.StandardUId == request.SelectedStandardId)?.Title;

        await UpdateCacheItem(request, amount);

        return new ApplicationRequest
        {
            EncodedAccountId = request.EncodedAccountId,
            EncodedPledgeId = request.EncodedPledgeId,
            CacheKey = request.CacheKey
        };
    }

    public async Task<GetFundingEstimateViewModel> GetFundingEstimate(GetFundingEstimateRequest request, GetApplicationDetailsResponse applicationDetails = null)
    {
        applicationDetails ??= await opportunitiesService.GetApplicationDetails(request.AccountId, request.PledgeId, request.SelectedStandardId);

        var amount = applicationDetails.Standards.Single()
            .ApprenticeshipFunding.GetEffectiveFundingLine(request.StartDate)
            .CalculateOneYearCost(request.NumberOfApprentices);

        return new GetFundingEstimateViewModel
        {
            Amount = amount,
            HasEnoughFunding = applicationDetails.Opportunity.RemainingAmount >= amount
        };
    }
}