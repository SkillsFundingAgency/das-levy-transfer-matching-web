using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Location;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class PledgeOrchestrator : IPledgeOrchestrator
    {
        private readonly ICacheStorageService _cacheStorageService;
        private readonly IPledgeService _pledgeService;
        private readonly IEncodingService _encodingService;
        private readonly ILocationValidatorService _validatorService;
        private readonly IUserService _userService;
        private Infrastructure.Configuration.FeatureToggles _featureToggles;
        private readonly IDateTimeService _dateTimeService;

        public PledgeOrchestrator(ICacheStorageService cacheStorageService, IPledgeService pledgeService, IEncodingService encodingService, ILocationValidatorService validatorService, IUserService userService, Infrastructure.Configuration.FeatureToggles featureToggles, IDateTimeService dateTimeService)
        {
            _cacheStorageService = cacheStorageService;
            _pledgeService = pledgeService;
            _encodingService = encodingService;
            _validatorService = validatorService;
            _userService = userService;
            _featureToggles = featureToggles;
            _dateTimeService = dateTimeService;
        }

        public InformViewModel GetInformViewModel(string encodedAccountId)
        {
            return new InformViewModel
            {
                EncodedAccountId = encodedAccountId,
                CacheKey = Guid.NewGuid()
            };
        }

        public async Task<PledgesViewModel> GetPledgesViewModel(PledgesRequest request)
        {
            var pledgesResponse = await _pledgeService.GetPledges(request.AccountId);
            var renderCreatePledgesButton = _userService.IsUserChangeAuthorized();
            
            return new PledgesViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                RenderCreatePledgeButton = renderCreatePledgesButton,
                Pledges = pledgesResponse.Pledges.Select(x => new PledgesViewModel.Pledge 
                {
                    ReferenceNumber = _encodingService.Encode(x.Id, EncodingType.PledgeId),
                    Amount = x.Amount,
                    RemainingAmount = x.RemainingAmount,
                    ApplicationCount = x.ApplicationCount
                })
            };
        }

        public DetailViewModel GetDetailViewModel(DetailRequest request)
        {
            return new DetailViewModel
            {
                EncodedPledgeId = request.EncodedPledgeId
            };
        }

        public async Task<CreateViewModel> GetCreateViewModel(CreateRequest request)
        {
            var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
            var dataTask = _pledgeService.GetCreate(request.AccountId);
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
                LevelOptions = dataTask.Result.Levels.ToList(),
                SectorOptions = dataTask.Result.Sectors.ToList(),
                JobRoleOptions = dataTask.Result.JobRoles.ToList(),
                Locations = cacheItem.Locations?.OrderBy(x => x).ToList()
            };
        }

        public async Task<AmountViewModel> GetAmountViewModel(AmountRequest request)
        {
            var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
            var accountDataTask = _pledgeService.GetAmount(request.EncodedAccountId);

            await Task.WhenAll(cacheItemTask, accountDataTask);
            var cacheItem = cacheItemTask.Result;
            var accountData = accountDataTask.Result;

            return new AmountViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                Amount = cacheItem.Amount.ToString(),
                RemainingTransferAllowance = accountData.RemainingTransferAllowance.ToString("N0"),
                IsNamePublic = cacheItem.IsNamePublic,
                DasAccountName = accountData.DasAccountName
            };
        }

        public async Task<SectorViewModel> GetSectorViewModel(SectorRequest request)
        {
            var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
            var sectorsTask = _pledgeService.GetSector(request.AccountId);

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
            var jobRolesTask = _pledgeService.GetJobRole(request.AccountId);

            await Task.WhenAll(cacheItemTask, jobRolesTask);

            return new JobRoleViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                CacheKey = request.CacheKey,
                JobRoles = cacheItemTask.Result.JobRoles,
                JobRoleOptions = jobRolesTask.Result.JobRoles.ToList()
            };
        }

        public async Task<string> SubmitPledge(CreatePostRequest request)
        {
            var cacheItem = await _cacheStorageService.RetrieveFromCache<CreatePledgeCacheItem>(request.CacheKey.ToString());

            ValidateCreatePledgeCacheItem(cacheItem);

            var createPledgeRequest = new CreatePledgeRequest
            {
                Amount = cacheItem.Amount.Value,
                IsNamePublic = cacheItem.IsNamePublic.Value,
                DasAccountName = cacheItem.DasAccountName,
                Sectors = cacheItem.Sectors ?? new List<string>(),
                JobRoles = cacheItem.JobRoles ?? new List<string>(),
                Levels = cacheItem.Levels ?? new List<string>(),
                Locations = cacheItem.Locations?.Where(x => x != null).ToList() ?? new List<string>(),
                UserId = _userService.GetUserId(),
                UserDisplayName = _userService.GetUserDisplayName()
            };

            var pledgeId = await _pledgeService.PostPledge(createPledgeRequest, request.AccountId);
            await _cacheStorageService.DeleteFromCache(request.CacheKey.ToString());

            return _encodingService.Encode(pledgeId, EncodingType.PledgeId);
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
        
        public async Task<Dictionary<int, string>> ValidateLocations(LocationPostRequest request)
        {
            return await _validatorService.ValidateLocations(request);
        }

        public async Task UpdateCacheItem(AmountPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.Amount = int.Parse(request.Amount, NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
            cacheItem.IsNamePublic = request.IsNamePublic.Value;
            cacheItem.DasAccountName = request.DasAccountName;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(SectorPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.Sectors = request.Sectors;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(JobRolePostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.JobRoles = request.JobRoles;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(LevelPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.Levels = request.Levels;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(LocationPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.Locations = request.Locations;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task<LevelViewModel> GetLevelViewModel(LevelRequest request)
        {
            var cacheItemTask = RetrievePledgeCacheItem(request.CacheKey);
            var levelsTask = _pledgeService.GetLevel(request.AccountId);

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
            var result = await _cacheStorageService.RetrieveFromCache<CreatePledgeCacheItem>(key.ToString());

            if (result == null)
            {
                result = new CreatePledgeCacheItem(key);
                await _cacheStorageService.SaveToCache(key.ToString(), result, 1);
            }

            return result;
        }

        private void ValidateCreatePledgeCacheItem(CreatePledgeCacheItem cacheItem)
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

        public async Task<ApplicationsViewModel> GetApplications(ApplicationsRequest request)
        {
            var result = await _pledgeService.GetApplications(request.AccountId, request.PledgeId);

            return new ApplicationsViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                Applications = result.Applications?.Select(app => new GetApplicationViewModel
                {
                    EncodedApplicationId = _encodingService.Encode(app.Id, EncodingType.PledgeApplicationId),
                    DasAccountName = app.DasAccountName,
                    Amount = app.Amount,
                    Duration = app.Standard.ApprenticeshipFunding.GetEffectiveFundingLine(app.StartDate).Duration,
                    CreatedOn = app.CreatedOn,
                    Status = "Awaiting approval"
                })
            };
        }

        public async Task<GetApplicationViewModel> GetApplicationViewModel(ApplicationRequest request, CancellationToken cancellationToken = default)
        {
            var result =
                await _pledgeService.GetApplication(request.AccountId, request.PledgeId, request.ApplicationId, cancellationToken);

            if (result != null)
            {
                return new GetApplicationViewModel
                {
                    AboutOpportunity = result.AboutOpportunity,
                    BusinessWebsite = result.BusinessWebsite,
                    EmailAddresses = result.EmailAddresses,
                    EmployerAccountName = result.EmployerAccountName,
                    EstimatedDurationMonths = result.EstimatedDurationMonths,
                    FirstName = result.FirstName,
                    HasTrainingProvider = result.HasTrainingProvider,
                    LastName = result.LastName,
                    NumberOfApprentices = result.NumberOfApprentices,
                    StartBy = result.StartBy,
                    Sectors = result.Sector,
                    AllSectors = result.AllSectors,
                    PledgeSectors = result.PledgeSectors,
                    PledgeJobRoles = result.PledgeJobRoles,
                    PledgeLevels = result.PledgeLevels,
                    PledgeLocations = result.PledgeLocations,
                    Location = result.Location,
                    JobRole = result.TypeOfJobRole,
                    Level = result.Level,
                    Affordability = GetAffordabilityViewModel(result.PledgeRemainingAmount, result.NumberOfApprentices, result.MaxFunding, result.EstimatedDurationMonths, result.StartBy)
                };
            }

            return null;
        }

        public GetApplicationViewModel.AffordabilityViewModel GetAffordabilityViewModel(int remainingAmount, int numberOfApprentices, int maxFunding, int estimatedDurationMonths, DateTime startDate)
        {
            int remainingFunds = remainingAmount;

            var netCost = maxFunding - (maxFunding * 0.2);
            var monthlyCost = netCost / estimatedDurationMonths;
            var estimatedCostThisYear = monthlyCost * startDate.MonthsTillFinancialYearEnd();

            var remainingFundsIfApproved = remainingFunds - estimatedCostThisYear;

            var estimatedCostOverDuration = maxFunding * numberOfApprentices * estimatedDurationMonths;

            return new GetApplicationViewModel.AffordabilityViewModel
            {
                RemainingFunds = remainingFunds.ToCurrencyString(),
                EstimatedCostThisYear = estimatedCostThisYear.ToCurrencyString(),
                RemainingFundsIfApproved = remainingFundsIfApproved.ToCurrencyString(),
                EstimatedCostOverDuration = estimatedCostOverDuration.ToCurrencyString(),
                YearDescription = _dateTimeService.UtcNow.ToTaxYearDescription()
            };
        }
    }
}