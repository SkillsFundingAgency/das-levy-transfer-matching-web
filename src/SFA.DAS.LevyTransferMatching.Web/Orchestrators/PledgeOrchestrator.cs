using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Services;
using SFA.DAS.LevyTransferMatching.Web.Validators.Location;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class PledgeOrchestrator : IPledgeOrchestrator
    {
        private const string LocationSelectionCacheItemPrefix = "LocationSelectionCacheItem";

        private readonly ICacheStorageService _cacheStorageService;
        private readonly IPledgeService _pledgeService;
        private readonly IEncodingService _encodingService;
        private readonly ILocationValidatorService _validatorService;
        private readonly IUserService _userService;
        private Infrastructure.Configuration.FeatureToggles _featureToggles;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICsvHelperService _csvService;

        public PledgeOrchestrator(ICacheStorageService cacheStorageService, IPledgeService pledgeService, IEncodingService encodingService, ILocationValidatorService validatorService, IUserService userService, Infrastructure.Configuration.FeatureToggles featureToggles, IDateTimeService dateTimeService, ICsvHelperService csvService)
        {
            _cacheStorageService = cacheStorageService;
            _pledgeService = pledgeService;
            _encodingService = encodingService;
            _validatorService = validatorService;
            _userService = userService;
            _featureToggles = featureToggles;
            _dateTimeService = dateTimeService;
            _csvService = csvService;
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
                Sectors = cacheItemTask.Result.Sectors,
                JobRoleOptions = jobRolesTask.Result.JobRoles.ToList(),
                SectorOptions = jobRolesTask.Result.Sectors.ToList()
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

        public async Task<ApplicationApprovedViewModel> GetApplicationApprovedViewModel(ApplicationApprovedRequest request)
        {
            var response = await _pledgeService.GetApplicationApproved(request.AccountId, request.PledgeId, request.ApplicationId);

            return new ApplicationApprovedViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                DasAccountName = response.EmployerAccountName,
                AllowTransferRequestAutoApproval = response.AutomaticApproval
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

        public async Task<byte[]> GetPledgeApplicationsDownloadModel(ApplicationsRequest request)
        {
            var result = await _pledgeService.GetApplications(request.AccountId, request.PledgeId);

            var pledgeAppModel = new PledgeApplicationsDownloadModel
            {
                Applications = result.Applications?.Select(app => new PledgeApplicationDownloadModel
                {
                    DateApplied = app.CreatedOn,
                    Status = app.Status,
                    ApplicationId = app.Id,
                    PledgeId = app.PledgeId,
                    EmployerAccountName = app.DasAccountName,
                    HasTrainingProvider = app.HasTrainingProvider,
                    Sectors = app.Sectors ?? new List<string>(),
                    AboutOpportunity = app.Details,
                    BusinessWebsite = GetUrlWithPrefix(app.BusinessWebsite),
                    FormattedEmailAddress = String.Join(";", app.EmailAddresses),
                    FormattedSectors = String.Join(",", app.Sectors ?? new List<string>()),
                    FirstName = app.FirstName,
                    LastName = app.LastName,
                    NumberOfApprentices = app.NumberOfApprentices,
                    StartBy = app.StartDate,
                    TypeOfJobRole = app.JobRole,
                    EncodedPledgeId = _encodingService.Encode(app.PledgeId, EncodingType.PledgeId),
                    EncodedApplicationId = _encodingService.Encode(app.Id, EncodingType.PledgeApplicationId),
                    IsJobRoleMatch = app.IsJobRoleMatch,
                    IsLevelMatch = app.IsLevelMatch,
                    IsLocationMatch = app.IsLocationMatch,
                    IsSectorMatch = app.IsSectorMatch,
                    Duration = app.StandardDuration,
                    EstimatedCostThisYear = app.Amount,
                    Level = app.Level,
                    TotalEstimatedCost = app.MaxFunding,
                    AdditionalLocations = app.AdditionalLocations,
                    SpecificLocation = app.SpecificLocation,
                    Locations = app.Locations,
                    PledgeLocations = app.PledgeLocations,
                    DynamicLocations = GetListOfLocations(app.PledgeLocations, app.Locations, app.SpecificLocation, app.AdditionalLocations)
                })
            };

            var fileContents = _csvService.GenerateCsvFileFromModel(pledgeAppModel);

            return fileContents;
        }

        private IEnumerable<dynamic> GetListOfLocations(IEnumerable<GetApplyResponse.PledgeLocation> pledgeLocations, IEnumerable<GetApplicationsResponse.ApplicationLocation> applicationLocations, string specificLocation, string additionalLocations)
        {
            var listOfMatchingLocations = (from location in applicationLocations
                select pledgeLocations?.FirstOrDefault(o => o.Id == location.PledgeLocationId)
                into matchedLocation
                where matchedLocation != null && !string.IsNullOrWhiteSpace(matchedLocation.Name)
                select matchedLocation.Name).ToList();

            if (!string.IsNullOrWhiteSpace(specificLocation))
            {
                listOfMatchingLocations.Add(specificLocation);
            }

            if (!string.IsNullOrWhiteSpace(additionalLocations))
            {
                listOfMatchingLocations.Add(additionalLocations);
            }

            var locations = new List<dynamic>();
            
            foreach (var matchingLocation in listOfMatchingLocations)
            {
                dynamic location = new ExpandoObject();
                location.Name = matchingLocation;
                locations.Add(location);
            }

            return locations;
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
            var errors = await _validatorService.ValidateLocations(request, multipleValidLocations);

            await UpdateCacheItem(request.CacheKey, multipleValidLocations);

            return errors;
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

            cacheItem.Locations = request.AllLocationsSelected ? null : request.Locations;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(Guid cacheKey, IDictionary<int, IEnumerable<string>> multipleValidLocations)
        {
            var cacheItem = await RetrieveLocationSelectionCacheItem(cacheKey);

            cacheItem.MultipleValidLocations = multipleValidLocations;

            await _cacheStorageService.SaveToCache($"{LocationSelectionCacheItemPrefix}_{cacheItem.Key}", cacheItem, 1);
        }

        public async Task UpdateCacheItem(LocationSelectPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            foreach (var locationSelectionGroup in request.SelectValidLocationGroups)
            {
                cacheItem.Locations[locationSelectionGroup.Index] = locationSelectionGroup.SelectedValue;
            }

            await _cacheStorageService.SaveToCache(request.CacheKey.ToString(), cacheItem, 1);
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

        private async Task<LocationSelectionCacheItem> RetrieveLocationSelectionCacheItem(Guid key)
        {
            var result = await _cacheStorageService.RetrieveFromCache<LocationSelectionCacheItem>($"{LocationSelectionCacheItemPrefix}_{key}");

            if (result == null)
            {
                result = new LocationSelectionCacheItem(key);
                await _cacheStorageService.SaveToCache($"{LocationSelectionCacheItemPrefix}_{key}", result, 1);
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

            var viewModels = (from application in result.Applications
                let pledgeApplication = result.Applications.First(x => x.PledgeId == application.PledgeId)
                              select new ApplicationViewModel
                              {
                                  EncodedApplicationId = _encodingService.Encode(application.Id, EncodingType.PledgeApplicationId),
                                  DasAccountName = application.DasAccountName,
                                  Amount = application.Amount,
                                  Duration = application.StandardDuration,
                                  CreatedOn = application.CreatedOn,
                                  Status = application.Status,
                                  IsLocationMatch = application.IsLocationMatch,
                                  IsSectorMatch = application.IsSectorMatch,
                                  IsJobRoleMatch = application.IsJobRoleMatch,
                                  IsLevelMatch = application.IsLevelMatch,
                                  StartBy = application.StartDate,
                                  BusinessWebsite = pledgeApplication.BusinessWebsite,
                                  LastName = pledgeApplication.LastName,
                                  FirstName = pledgeApplication.FirstName,
                                  EmailAddresses = pledgeApplication.EmailAddresses,
                                  JobRole = pledgeApplication.JobRole,
                                  PledgeRemainingAmount = pledgeApplication.PledgeRemainingAmount,
                                  MaxFunding = pledgeApplication.MaxFunding,
                                  Details = pledgeApplication.Details
                              }).ToList();

            return new ApplicationsViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                DisplayRejectedBanner = request.DisplayRejectedBanner,
                RejectedEmployerName = request.RejectedEmployerName,
                Applications = viewModels
            };
        }

        public async Task<ApplicationViewModel> GetApplicationViewModel(ApplicationRequest request, CancellationToken cancellationToken = default)
        {
            var result =
                await _pledgeService.GetApplication(request.AccountId, request.PledgeId, request.ApplicationId, cancellationToken);

            var isOwnerOrTransactor = _userService.IsOwnerOrTransactor(request.AccountId);

            if (result != null)
            {
                return new ApplicationViewModel
                {
                    AboutOpportunity = result.AboutOpportunity,
                    BusinessWebsite = GetUrlWithPrefix(result.BusinessWebsite),
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
                    JobRole = result.TypeOfJobRole,
                    Level = result.Level,
                    DisplaySectors = result.Sector.ToReferenceDataDescriptionList(result.AllSectors, "; "),
                    Locations = string.IsNullOrEmpty(result.SpecificLocation) ? result.Locations.ToApplicationLocationsString(", ", result.AdditionalLocation) : result.SpecificLocation,
                    IsLocationMatch = (result.Locations != null && result.Locations.Any()) || !result.PledgeLocations.Any(),
                    Affordability = GetAffordabilityViewModel(result.Amount, result.PledgeRemainingAmount, result.NumberOfApprentices, result.MaxFunding, result.EstimatedDurationMonths, result.StartBy),
                    AllowApproval = result.Status == ApplicationStatus.Pending && result.Amount <= result.PledgeRemainingAmount && isOwnerOrTransactor,
                    DisplayApplicationApprovalOptions = _featureToggles.FeatureToggleApplicationApprovalOptions,
					AllowTransferRequestAutoApproval = result.AutomaticApproval
                };
            }

            return null;
        }

        public async Task<ApplicationApprovalOptionsViewModel> GetApplicationApprovalOptionsViewModel(ApplicationApprovalOptionsRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _pledgeService.GetApplicationApprovalOptions(request.AccountId, request.PledgeId, request.ApplicationId, cancellationToken);

            return new ApplicationApprovalOptionsViewModel
            {
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                EncodedApplicationId = request.EncodedApplicationId,
                EmployerAccountName = response.EmployerAccountName,
                IsApplicationPending = response.ApplicationStatus == ApplicationStatus.Pending
            };
        }

        public async Task SetApplicationApprovalOptions(ApplicationApprovalOptionsPostRequest request, CancellationToken cancellationToken = default)
        {
            var serviceRequest = new SetApplicationApprovalOptionsRequest
            {
                UserId = _userService.GetUserId(),
                UserDisplayName = _userService.GetUserDisplayName(),
                AutomaticApproval = request.AutomaticApproval.Value
            };

            await _pledgeService.SetApplicationApprovalOptions(request.AccountId, request.ApplicationId, request.PledgeId, serviceRequest);
        }

        public async Task SetApplicationOutcome(ApplicationPostRequest request)
        {
            var outcomeRequest = new SetApplicationOutcomeRequest
            {
                UserId = _userService.GetUserId(),
                UserDisplayName = _userService.GetUserDisplayName(),
                Outcome = request.SelectedAction == ApplicationPostRequest.ApprovalAction.Approve
                    ? SetApplicationOutcomeRequest.ApplicationOutcome.Approve
                    : SetApplicationOutcomeRequest.ApplicationOutcome.Reject
            };

            await _pledgeService.SetApplicationOutcome(request.AccountId, request.ApplicationId, request.PledgeId, outcomeRequest);
        }

        private string GetUrlWithPrefix(string url)
        {
            if (String.IsNullOrWhiteSpace(url)) return url;

            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                return url;
            }

            return $"http://{url}";
        }

        public ApplicationViewModel.AffordabilityViewModel GetAffordabilityViewModel(int amount, int remainingAmount, int numberOfApprentices, int maxFunding, int estimatedDurationMonths, DateTime startDate)
        {
            var remainingFundsIfApproved = remainingAmount - amount;
            var estimatedCostOverDuration = maxFunding * numberOfApprentices;

            return new ApplicationViewModel.AffordabilityViewModel
            {
                RemainingFunds = remainingAmount.ToCurrencyString(),
                EstimatedCostThisYear = amount.ToCurrencyString(),
                RemainingFundsIfApproved = remainingFundsIfApproved.ToCurrencyString(),
                EstimatedCostOverDuration = estimatedCostOverDuration.ToCurrencyString(),
                YearDescription = _dateTimeService.UtcNow.ToTaxYearDescription()
            };
        }
    }
}