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

        public OpportunitiesOrchestrator(IDateTimeService dateTimeService, IOpportunitiesService opportunitiesService, ITagService tagService, IUserService userService, IEncodingService encodingService, ICacheStorageService cacheStorageService)
        {
            _dateTimeService = dateTimeService;
            _opportunitiesService = opportunitiesService;
            _tagService = tagService;
            _encodingService = encodingService;
            _tagService = tagService;
            _userService = userService;
            _cacheStorageService = cacheStorageService;
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

            return new ApplyViewModel
            {
                CacheKey = application.Key,
                EncodedPledgeId = request.EncodedPledgeId,
                EncodedAccountId = request.EncodedAccountId,
                OpportunitySummaryViewModel = await GetOpportunitySummaryViewModel(opportunityDto, request.EncodedPledgeId),
                JobRole = "-",
                NumberOfApprentices = "-",
                StartBy = "-",
                HaveTrainingProvider = "-",
                Sectors = "-",
                Locations = "-",
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

        public async Task UpdateCacheItem(MoreDetailsPostRequest request)
        {
            var cacheItem = await RetrieveCacheItem(request.CacheKey);

            cacheItem.Details = request.Details;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        public async Task UpdateCacheItem(ApplicationDetailsPostRequest request)
        {
            var cacheItem = await RetrieveCacheItem(request.CacheKey);

            cacheItem.JobRoles = request.JobRoles;
            cacheItem.NumberOfApprentices = request.NumberOfApprentices;
            cacheItem.StartDate = request.StartDate;
            cacheItem.HasTrainingProvider = request.HasTrainingProvider;

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
            var application = await RetrieveCacheItem(request.CacheKey);
            var opportunityDto = await _opportunitiesService.GetOpportunity((int)_encodingService.Decode(request.EncodedPledgeId, EncodingType.PledgeId));

            return new ApplicationDetailsViewModel()
            {
                CacheKey = request.CacheKey,
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                JobRoles = application.JobRoles,
                NumberOfApprentices = application.NumberOfApprentices,
                StartDate = application.StartDate,
                HasTrainingProvider = application.HasTrainingProvider,
                OpportunitySummaryViewModel = new OpportunitySummaryViewModel
                {
                    Description = GenerateDescription(opportunityDto, request.EncodedPledgeId),
                    Amount = opportunityDto.Amount,
                    JobRoleList = string.Join(", ", opportunityDto.JobRoles),
                    LevelList = string.Join(", ", opportunityDto.Levels),
                    SectorList = string.Join(", ", opportunityDto.Sectors),
                    YearDescription = "2021/22"
                }
            };
        }
    }
}