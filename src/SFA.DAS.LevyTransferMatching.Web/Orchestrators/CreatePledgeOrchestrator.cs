﻿using System.Globalization;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Location;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators;

public interface ICreatePledgeOrchestrator
{
    InformViewModel GetInformViewModel(string encodedAccountId);
    Task<CreateViewModel> GetCreateViewModel(CreateRequest request);
    Task<AmountViewModel> GetAmountViewModel(AmountRequest request);
    Task<OrganisationNameViewModel> GetOrganisationNameViewModel(OrganisationNameRequest request);
    Task<AutoApproveViewModel> GetAutoApproveViewModel(AutoApproveRequest request);
    Task<SectorViewModel> GetSectorViewModel(SectorRequest request);
    Task<JobRoleViewModel> GetJobRoleViewModel(JobRoleRequest request);
    Task<LocationViewModel> GetLocationViewModel(LocationRequest request);
    Task<LevelViewModel> GetLevelViewModel(LevelRequest request);
    Task<LocationSelectViewModel> GetLocationSelectViewModel(LocationSelectRequest request);
    Task<Dictionary<int, string>> ValidateLocations(LocationPostRequest request, IDictionary<int, IEnumerable<string>> multipleValidLocations);
    Task UpdateCacheItem(AmountPostRequest request);
    Task UpdateCacheItem(AutoApprovePostRequest request);
    Task UpdateCacheItem(OrganisationNamePostRequest request);
    Task UpdateCacheItem(SectorPostRequest request);
    Task UpdateCacheItem(JobRolePostRequest request);
    Task UpdateCacheItem(LevelPostRequest request);
    Task UpdateCacheItem(LocationSelectPostRequest request);
    Task UpdateCacheItem(LocationPostRequest request);
    Task<string> CreatePledge(CreatePostRequest request);
}

public class CreatePledgeOrchestrator(
    ICacheStorageService cacheStorageService,
    IPledgeService pledgeService,
    IEncodingService encodingService,
    ILocationValidatorService validatorService,
    IUserService userService,
    Infrastructure.Configuration.FeatureToggles featureToggles)
    : ICreatePledgeOrchestrator
{
    private const string LocationSelectionCacheItemPrefix = "LocationSelectionCacheItem";

    public InformViewModel GetInformViewModel(string encodedAccountId)
    {
        return new InformViewModel
        {
            EncodedAccountId = encodedAccountId,
            CacheKey = Guid.NewGuid()
        };
    }

    public async Task<CreateViewModel> GetCreateViewModel(CreateRequest request)
    {
        var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
        var dataTask = pledgeService.GetCreate(request.AccountId);
        await Task.WhenAll(cacheItemTask, dataTask);

        var cacheItem = cacheItemTask.Result;

        return new CreateViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            CacheKey = request.CacheKey,
            Amount = cacheItem.Amount,
            IsNamePublic = cacheItem.IsNamePublic,
            Sectors = cacheItem.Sectors,
            JobRoles = cacheItem.JobRoles,
            Levels = cacheItem.Levels,
            AutomaticApprovalOption = cacheItem.AutomaticApprovalOption,
            LevelOptions = dataTask.Result.Levels.ToList(),
            SectorOptions = dataTask.Result.Sectors.ToList(),
            JobRoleOptions = dataTask.Result.JobRoles.ToList(),
            Locations = cacheItem.Locations?.OrderBy(x => x).ToList(),
            AutoApprovalIsEnabled = featureToggles.FeatureToggleApplicationAutoApprove
        };
    }

    public async Task<AmountViewModel> GetAmountViewModel(AmountRequest request)
    {
        var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
        var accountDataTask = pledgeService.GetAmount(request.EncodedAccountId);

        await Task.WhenAll(cacheItemTask, accountDataTask);
        var cacheItem = cacheItemTask.Result;
        var accountData = accountDataTask.Result;

        return new AmountViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            CacheKey = request.CacheKey,
            Amount = cacheItem.Amount.ToString(),
            RemainingTransferAllowance = accountData.RemainingTransferAllowance.ToString("N0"),
            StartingTransferAllowance = accountData.StartingTransferAllowance,
            FinancialYearString = DateTime.UtcNow.Year.ToString()
        };
    }

    public async Task<OrganisationNameViewModel> GetOrganisationNameViewModel(OrganisationNameRequest request)
    {
        var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
        var accountDataTask = pledgeService.GetOrganisationName(request.EncodedAccountId);

        await Task.WhenAll(cacheItemTask, accountDataTask);
        var cacheItem = cacheItemTask.Result;
        var accountData = accountDataTask.Result;

        return new OrganisationNameViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            CacheKey = request.CacheKey,
            IsNamePublic = cacheItem.IsNamePublic,
            DasAccountName = accountData.DasAccountName
        };
    }

    public async Task<AutoApproveViewModel> GetAutoApproveViewModel(AutoApproveRequest request)
    {
        var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

        return new AutoApproveViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            CacheKey = request.CacheKey,
            AutomaticApprovalOption = cacheItem.AutomaticApprovalOption
        };
    }

    public async Task<SectorViewModel> GetSectorViewModel(SectorRequest request)
    {
        var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
        var sectorsTask = pledgeService.GetSector(request.AccountId);

        await Task.WhenAll(cacheItemTask, sectorsTask);

        return new SectorViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            CacheKey = request.CacheKey,
            Sectors = cacheItemTask.Result.Sectors,
            SectorOptions = sectorsTask.Result.Sectors.ToList()
        };
    }

    public async Task<JobRoleViewModel> GetJobRoleViewModel(JobRoleRequest request)
    {
        var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
        var jobRolesTask = pledgeService.GetJobRole(request.AccountId);

        await Task.WhenAll(cacheItemTask, jobRolesTask);

        return new JobRoleViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            CacheKey = request.CacheKey,
            JobRoles = cacheItemTask.Result.JobRoles,
            Sectors = cacheItemTask.Result.Sectors,
            JobRoleOptions = jobRolesTask.Result.JobRoles.ToList(),
            SectorOptions = jobRolesTask.Result.Sectors.ToList()
        };
    }

    public async Task<string> CreatePledge(CreatePostRequest request)
    {
        var cacheItem = await cacheStorageService.RetrieveFromCache<CreatePledgeCacheItem>(request.CacheKey.ToString());

        ValidateCreatePledgeCacheItem(cacheItem);

        var createPledgeRequest = new CreatePledgeRequest
        {
            Amount = cacheItem.Amount.Value,
            IsNamePublic = cacheItem.IsNamePublic.Value,
            DasAccountName = cacheItem.DasAccountName,
            Sectors = cacheItem.Sectors ?? [],
            JobRoles = cacheItem.JobRoles ?? [],
            Levels = cacheItem.Levels ?? [],
            Locations = cacheItem.Locations?.Where(x => x != null).ToList() ?? [],
            UserId = userService.GetUserId(),
            UserDisplayName = userService.GetUserDisplayName(),
            AutomaticApprovalOption = cacheItem.AutomaticApprovalOption,
        };

        var pledgeId = await pledgeService.PostPledge(createPledgeRequest, request.AccountId);
        await cacheStorageService.DeleteFromCache(request.CacheKey.ToString());

        return encodingService.Encode(pledgeId, EncodingType.PledgeId);
    }

    public async Task<LocationViewModel> GetLocationViewModel(LocationRequest request)
    {
        var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

        return new LocationViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            CacheKey = request.CacheKey,
            Locations = cacheItem.Locations?.ToList()
        };
    }


    public async Task<LocationSelectViewModel> GetLocationSelectViewModel(LocationSelectRequest request)
    {
        var cacheItem = await RetrieveLocationSelectionCacheItem(request.CacheKey);

        var selectValidLocationGroups = cacheItem.MultipleValidLocations
            .Select(MapValidLocationGroup)
            .ToArray();

        return new LocationSelectViewModel()
        {
            CacheKey = request.CacheKey,
            EncodedAccountId = request.EncodedAccountId,
            SelectValidLocationGroups = selectValidLocationGroups,
        };
    }


    private LocationSelectPostRequest.SelectValidLocationGroup MapValidLocationGroup(KeyValuePair<int, IEnumerable<string>> kvp)
    {
        return new LocationSelectPostRequest.SelectValidLocationGroup()
        {
            Index = kvp.Key,
            ValidLocationItems = kvp.Value
                .Select(y => new LocationSelectPostRequest.SelectValidLocationGroup.ValidLocationItem()
                {
                    Value = y,
                })
                .ToArray()
        };
    }

    public async Task<Dictionary<int, string>> ValidateLocations(LocationPostRequest request, IDictionary<int, IEnumerable<string>> multipleValidLocations)
    {
        var errors = await validatorService.ValidateLocations(request, multipleValidLocations);

        await UpdateCacheItem(request.CacheKey, multipleValidLocations);

        return errors;
    }

    public async Task UpdateCacheItem(AmountPostRequest request)
    {
        var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

        cacheItem.Amount = int.Parse(request.Amount, NumberStyles.AllowThousands, CultureInfo.InvariantCulture);

        await cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
    }

    public async Task UpdateCacheItem(OrganisationNamePostRequest request)
    {
        var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

        cacheItem.DasAccountName = request.DasAccountName;
        cacheItem.IsNamePublic = request.IsNamePublic;

        await cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
    }

    public async Task UpdateCacheItem(AutoApprovePostRequest request)
    {
        var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

        cacheItem.AutomaticApprovalOption = request.AutomaticApprovalOption;

        await cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
    }

    public async Task UpdateCacheItem(SectorPostRequest request)
    {
        var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

        cacheItem.Sectors = request.Sectors;

        await cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
    }

    public async Task UpdateCacheItem(JobRolePostRequest request)
    {
        var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

        cacheItem.JobRoles = request.JobRoles;

        await cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
    }

    public async Task UpdateCacheItem(LevelPostRequest request)
    {
        var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

        cacheItem.Levels = request.Levels;

        await cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
    }

    public async Task UpdateCacheItem(LocationPostRequest request)
    {
        var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

        cacheItem.Locations = request.AllLocationsSelected ? null : request.Locations;

        await cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
    }

    private async Task UpdateCacheItem(Guid cacheKey, IDictionary<int, IEnumerable<string>> multipleValidLocations)
    {
        var cacheItem = await RetrieveLocationSelectionCacheItem(cacheKey);

        cacheItem.MultipleValidLocations = multipleValidLocations;

        await cacheStorageService.SaveToCache($"{LocationSelectionCacheItemPrefix}_{cacheItem.Key}", cacheItem, 1);
    }

    public async Task UpdateCacheItem(LocationSelectPostRequest request)
    {
        var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

        foreach (var locationSelectionGroup in request.SelectValidLocationGroups)
        {
            cacheItem.Locations[locationSelectionGroup.Index] = locationSelectionGroup.SelectedValue;
        }

        await cacheStorageService.SaveToCache(request.CacheKey.ToString(), cacheItem, 1);
    }

    public async Task<LevelViewModel> GetLevelViewModel(LevelRequest request)
    {
        var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
        var levelsTask = pledgeService.GetLevel(request.AccountId);

        await Task.WhenAll(cacheItemTask, levelsTask);

        return new LevelViewModel
        {
            EncodedAccountId = request.EncodedAccountId,
            CacheKey = request.CacheKey,
            Levels = cacheItemTask.Result.Levels,
            LevelOptions = levelsTask.Result.Levels.ToList()
        };
    }

    private async Task<CreatePledgeCacheItem> RetrievePledgeCacheItem(Guid key)
    {
        var result = await cacheStorageService.RetrieveFromCache<CreatePledgeCacheItem>(key.ToString());

        if (result == null)
        {
            result = new CreatePledgeCacheItem(key);
            await cacheStorageService.SaveToCache(key.ToString(), result, 1);
        }

        return result;
    }

    private async Task<LocationSelectionCacheItem> RetrieveLocationSelectionCacheItem(Guid key)
    {
        var result = await cacheStorageService.RetrieveFromCache<LocationSelectionCacheItem>($"{LocationSelectionCacheItemPrefix}_{key}");

        if (result == null)
        {
            result = new LocationSelectionCacheItem(key);
            await cacheStorageService.SaveToCache($"{LocationSelectionCacheItemPrefix}_{key}", result, 1);
        }

        return result;
    }

    private static void ValidateCreatePledgeCacheItem(CreatePledgeCacheItem cacheItem)
    {
        if (cacheItem == null)
        {
            throw new InvalidOperationException("Unable to submit pledge due to cache expiry");
        }

        if (!cacheItem.Amount.HasValue)
        {
            throw new InvalidOperationException("Unable to submit pledge due to null cache value for pledge Amount");
        }

        if (!cacheItem.IsNamePublic.HasValue)
        {
            throw new InvalidOperationException("Unable to submit pledge due to null cache value for pledge IsNamePublic");
        }
    }
}