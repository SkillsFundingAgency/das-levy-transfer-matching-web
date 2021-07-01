using System;
using System.Data;
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
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class OpportunitiesOrchestrator : IOpportunitiesOrchestrator
    {
        private const string All = "All";

        private readonly ICacheStorageService _cacheStorageService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IOpportunitiesService _opportunitiesService;
        private readonly IEncodingService _encodingService;
        private readonly ITagService _tagService;
        private readonly IUserService _userService;

        public OpportunitiesOrchestrator(IDateTimeService dateTimeService, IOpportunitiesService opportunitiesService, ITagService tagService, IUserService userService, IEncodingService encodingService, ICacheStorageService cacheStorageService)
        {
            _dateTimeService = dateTimeService;
            _opportunitiesService = opportunitiesService;
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
            var opportunitiesDto = await _opportunitiesService.GetAllOpportunities();
            var opportunities = opportunitiesDto.Select(x => new Opportunity
                {
                    EmployerName = x.DasAccountName,
                    ReferenceNumber = _encodingService.Encode(x.Id, EncodingType.PledgeId),
                })
                .ToList();

            return new IndexViewModel { Opportunities = opportunities };
        }

        public async Task<string> GetUserEncodedAccountId(string userId)
        {
            var accounts = await _userService.GetUserAccounts(userId);

            // TODO: Below is temporary -
            //       Raised as an issue, and eventually to be replaced with
            //       an accounts selection screen.
            var firstEncodedAccountId = accounts
                .Select(x => x.EncodedAccountId)
                .First();

            return firstEncodedAccountId;
        }

        public async Task<OpportunitySummaryViewModel> GetOpportunitySummaryViewModel(OpportunityDto opportunityDto, string encodedPledgeId)
        {
            // Pull back the tags, and use the descriptions to build the lists.
            var sectorTags = await _tagService.GetSectors();
            string sectorList = opportunityDto.Sectors.ToTagDescriptionList(sectorTags);

            var jobRoleTags = await _tagService.GetJobRoles();
            string jobRoleList = opportunityDto.JobRoles.ToTagDescriptionList(jobRoleTags);

            var levelTags = await _tagService.GetLevels();

            bool allContainLevel = levelTags.All(x => x.TagId.Contains("Level"));
            if (allContainLevel)
            {
                levelTags.ForEach(x =>
                {
                    // Override the description property with the descriptions
                    // required in this instance.
                    x.Description = x.TagId.Replace("Level", string.Empty);
                });
            }
            else
            {
                // Note: Levels are different in how they're displayed here.
                //       It's been requested that only the number is shown
                //       here.
                //       If an additional tag is introduced that doesn't follow
                //       the "Level2", "Level3", structure, then this will
                //       break, and something fancier should probably be
                //       conisdered.
                throw new DataException("Unexpected level ID format detected in opportunity levels list. See comment above for more information.");
            }

            string levelList = opportunityDto.Levels.ToTagDescriptionList(levelTags);

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
            var application = await RetrievePledgeCacheItem(request.CacheKey);
            var opportunityDto = await _opportunitiesService.GetOpportunity((int)_encodingService.Decode(request.EncodedPledgeId, EncodingType.PledgeId));

            return new ApplyViewModel
            {
                CacheKey = application.Key,
                EncodedPledgeId = request.EncodedPledgeId,
                EncodedAccountId = request.EncodedAccountId,
                OpportunitySummaryViewModel = new OpportunitySummaryViewModel
                {
                    Description = GenerateDescription(opportunityDto, request.EncodedPledgeId),
                    Amount = opportunityDto.Amount,
                    JobRoleList = string.Join(", ", opportunityDto.JobRoles),
                    LevelList = string.Join(", ", opportunityDto.Levels),
                    SectorList = string.Join(", ", opportunityDto.Sectors),
                    YearDescription = "2021/22"
                },
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
            var application = await RetrievePledgeCacheItem(request.CacheKey);
            var opportunityDto = await _opportunitiesService.GetOpportunity((int)_encodingService.Decode(request.EncodedPledgeId, EncodingType.PledgeId));

            return new MoreDetailsViewModel()
            {
                CacheKey = request.CacheKey,
                EncodedAccountId = request.EncodedAccountId,
                EncodedPledgeId = request.EncodedPledgeId,
                Details = application.Details,
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

        public async Task UpdateCacheItem(MoreDetailsPostRequest request)
        {
            var cacheItem = await RetrievePledgeCacheItem(request.CacheKey);

            cacheItem.Details = request.Details;

            await _cacheStorageService.SaveToCache(cacheItem.Key.ToString(), cacheItem, 1);
        }

        private async Task<CreateApplicationCacheItem> RetrievePledgeCacheItem(Guid key)
        {
            var result = await _cacheStorageService.RetrieveFromCache<CreateApplicationCacheItem>(key.ToString());

            if (result == null)
            {
                result = new CreateApplicationCacheItem(key);
                await _cacheStorageService.SaveToCache(key.ToString(), result, 1);
            }

            return result;
        }
    }
}