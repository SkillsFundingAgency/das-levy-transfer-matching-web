using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.UserService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;
using FluentValidation;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class OpportunitiesOrchestrator : IOpportunitiesOrchestrator
    {
        private readonly ICacheStorageService _cacheStorageService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IOpportunitiesService _opportunitiesService;
        private readonly ITagService _tagService;
        private readonly IEncodingService _encodingService;
        private readonly IUserService _userService;
        private readonly ISectorPostRequestValidator _sectorValidator;

        public OpportunitiesOrchestrator(IDateTimeService dateTimeService, IOpportunitiesService opportunitiesService, ITagService tagService, IUserService userService, IEncodingService encodingService, ICacheStorageService cacheStorageService, ISectorPostRequestValidator sectorValidator)
        {
            _dateTimeService = dateTimeService;
            _opportunitiesService = opportunitiesService;
            _tagService = tagService;
            _encodingService = encodingService;
            _tagService = tagService;
            _userService = userService;
            _cacheStorageService = cacheStorageService;
            _sectorValidator = sectorValidator;
        }

        public async Task<DetailViewModel> GetDetailViewModel(int pledgeId)
        {
            var opportunityDto = await _opportunitiesService.GetOpportunity(pledgeId);

            if (opportunityDto == null)
            {
                return null;
            }

            var encodedPledgeId = _encodingService.Encode(opportunityDto.Id, EncodingType.PledgeId);

            var opportunitySummaryViewModel = await GetOpportunitySummaryViewModel(opportunityDto, encodedPledgeId);

            return new DetailViewModel()
            {
                EmployerName = opportunityDto.DasAccountName,
                EncodedPledgeId = encodedPledgeId,
                IsNamePublic = opportunityDto.IsNamePublic,
                OpportunitySummaryView = opportunitySummaryViewModel,
            };
        }

        public async Task<IndexViewModel> GetIndexViewModel()
        {
            var opportunitiesDto = _opportunitiesService.GetAllOpportunities();
            var levelsTask = _tagService.GetLevels();
            var sectorsTask = _tagService.GetSectors();
            var jobRolesTask = _tagService.GetJobRoles();

            await Task.WhenAll(opportunitiesDto, levelsTask, sectorsTask, jobRolesTask);

            List<Opportunity> opportunities = opportunitiesDto.Result
                .Select(x => new Opportunity
                {
                    Amount = x.Amount,
                    EmployerName = x.DasAccountName,
                    ReferenceNumber = _encodingService.Encode(x.Id, EncodingType.PledgeId),
                    Sectors = sectorsTask.Result.Where(y => x.Sectors.Contains(y.Id)).Select(y => y.Description).ToList(),
                    JobRoles = jobRolesTask.Result.Where(y => x.JobRoles.Contains(y.Id)).Select(y => y.Description).ToList(),
                    Levels = levelsTask.Result.Where(y => x.Levels.Contains(y.Id)).Select(y => y.ShortDescription).ToList(),
                    Locations = x.Locations
                }).ToList();

            return new IndexViewModel 
            { 
                Opportunities = opportunities
            };
        }

        public async Task<string> GetUserEncodedAccountId()
        {
            var userAccounts = await _userService.GetLoggedInUserAccounts();

            // TODO: Below is temporary -
            //       Raised as an issue, and eventually to be replaced with
            //       an accounts selection screen.
            var firstEncodedAccountId = userAccounts
                .Select(x => x.EncodedAccountId)
                .First();

            return firstEncodedAccountId;
        }

        public async Task<OpportunitySummaryViewModel> GetOpportunitySummaryViewModel(OpportunityDto opportunityDto, string encodedPledgeId)
        {
            // Pull back the tags, and use the descriptions to build the lists.
            var sectorReferenceDataItems = await _tagService.GetSectors();
            string sectorList = opportunityDto.Sectors.ToReferenceDataDescriptionList(sectorReferenceDataItems);

            var jobRoleReferenceDataItems = await _tagService.GetJobRoles();
            string jobRoleList = opportunityDto.JobRoles.ToReferenceDataDescriptionList(jobRoleReferenceDataItems);

            var levelReferenceDataItems = await _tagService.GetLevels();
            string levelList = opportunityDto.Levels.ToReferenceDataDescriptionList(levelReferenceDataItems, descriptionSource: x => x.ShortDescription);

            DateTime dateTime = _dateTimeService.UtcNow;

            return new OpportunitySummaryViewModel()
            {
                Amount = opportunityDto.Amount,
                Description = GenerateDescription(opportunityDto, encodedPledgeId),
                JobRoleList = jobRoleList,
                LevelList = levelList,
                SectorList = sectorList,
                YearDescription = dateTime.ToTaxYearDescription(),
            };
        }

        public async Task<ApplyViewModel> GetApplyViewModel(ApplicationRequest request)
        {
            var application = await RetrieveCacheItem(request.CacheKey);
            var opportunityDto = await _opportunitiesService.GetOpportunity(request.PledgeId);
            var sectorOptions = await _tagService.GetSectors();

            return new ApplyViewModel
            {
                CacheKey = application.Key,
                EncodedPledgeId = request.EncodedPledgeId,
                EncodedAccountId = request.EncodedAccountId,
                OpportunitySummaryViewModel = await GetOpportunitySummaryViewModel(opportunityDto, request.EncodedPledgeId),
                JobRole = application.JobRole ?? "-",
                NumberOfApprentices = application.NumberOfApprentices.HasValue ? application.NumberOfApprentices.Value.ToString() : "-",
                StartBy = application.StartDate.HasValue ? application.StartDate.Value.ToShortDisplayString() : "-",
                HaveTrainingProvider = application.HasTrainingProvider.HasValue ? "Yes" : "-",
                Sectors = application.Sectors?.ToList(),
                SectorOptions = sectorOptions,
                Location = application.Location?? "-",
                MoreDetail = application.Details ?? "-",
                ContactName = "-",
                EmailAddress = "-",
                WebsiteUrl = "-"
            };
        }

        private string GenerateDescription(OpportunityDto opportunityDto, string encodedPledgeId) => opportunityDto.IsNamePublic ? $"{opportunityDto.DasAccountName} ({encodedPledgeId})" : "A levy-paying business wants to fund apprenticeship training in:";

        public async Task<MoreDetailsViewModel> GetMoreDetailsViewModel(MoreDetailsRequest request)
        {
            var application = await RetrieveCacheItem(request.CacheKey);
            var opportunityDto = await _opportunitiesService.GetOpportunity((int)_encodingService.Decode(request.EncodedPledgeId, EncodingType.PledgeId));

            return new MoreDetailsViewModel()
            {
                CacheKey = request.CacheKey,
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                Details = application.Details,
                OpportunitySummaryViewModel = await GetOpportunitySummaryViewModel(opportunityDto, request.EncodedPledgeId),
            };
        }

        public async Task<SectorViewModel> GetSectorViewModel(SectorRequest request)
        {
            var cacheItem = await RetrieveCacheItem(request.CacheKey);
            var response = await _opportunitiesService.GetSector(request.AccountId, request.PledgeId);

            return new SectorViewModel
            {
                CacheKey = request.CacheKey,
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                Sectors = cacheItem.Sectors,
                SectorOptions = response.Sectors.ToList(),
                OpportunitySummaryViewModel = await GetOpportunitySummaryViewModel(response.Opportunity, request.EncodedPledgeId),
                Postcode = cacheItem.Postcode
            };
        }

        public async Task UpdateCacheItem(MoreDetailsPostRequest request)
        {
            var cacheItem = await RetrieveCacheItem(request.CacheKey);

            cacheItem.Details = request.Details;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(ApplicationDetailsPostRequest request)
        {
            var cacheItem = await RetrieveCacheItem(request.CacheKey);

            cacheItem.JobRole = request.SelectedStandardTitle;
            cacheItem.StandardId = request.SelectedStandardId;
            cacheItem.NumberOfApprentices = request.NumberOfApprentices.Value;
            cacheItem.StartDate = request.StartDate;
            cacheItem.HasTrainingProvider = request.HasTrainingProvider.Value;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(SectorPostRequest request)
        {
            var validationResult = _sectorValidator.Validate(request);

            var response = new GetSectorResponse();
            if(!validationResult.Errors.Any(x => x.PropertyName == "Postcode"))
            {
                response = await _opportunitiesService.GetSector(request.AccountId, request.PledgeId, request.Postcode);

                if(response?.Location == null)
                {
                    validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Postcode", "Enter a postcode"));
                }
            }

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var cacheItem = await RetrieveCacheItem(request.CacheKey);

            cacheItem.Sectors = request.Sectors;
            cacheItem.Location = response.Location;
            cacheItem.Postcode = request.Postcode.ToUpper();

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        private async Task<CreateApplicationCacheItem> RetrieveCacheItem(Guid key)
        {
            var result = await _cacheStorageService.RetrieveFromCache<CreateApplicationCacheItem>(key.ToString());

            if (result == null)
            {
                result = new CreateApplicationCacheItem(key);
                await _cacheStorageService.SaveToCache(key.ToString(), result, 1);
            }

            return result;
        }

        public async Task<ApplicationDetailsViewModel> GetApplicationViewModel(ApplicationDetailsRequest request)
        {
            var applicationDetailsTask = _opportunitiesService.GetApplicationDetails(request.PledgeId);
            var applicationTask = RetrieveCacheItem(request.CacheKey);
            var sectorReferenceDataItemsTask = _tagService.GetSectors();
            var jobRoleReferenceDataItemsTask = _tagService.GetJobRoles();
            var levelReferenceDataItemsTask = _tagService.GetLevels();

            await Task.WhenAll(applicationDetailsTask, applicationTask, sectorReferenceDataItemsTask, jobRoleReferenceDataItemsTask, levelReferenceDataItemsTask);

            var application = applicationTask.Result;
            var applicationDetails = applicationDetailsTask.Result;

            return new ApplicationDetailsViewModel()
            {
                CacheKey = request.CacheKey,
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                JobRole = application.JobRole,
                NumberOfApprentices = application.NumberOfApprentices,
                Month = application.StartDate?.Month,
                Year = application.StartDate?.Year,
                HasTrainingProvider = application.HasTrainingProvider,
                OpportunitySummaryViewModel = await GetOpportunitySummaryViewModel(applicationDetails.Opportunity, request.EncodedPledgeId),
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
                        Selected = !string.IsNullOrEmpty(application.StandardId) && (app.StandardUId == application.StandardId) ? "selected": null
                    })
                }
            };
        }
    }
}