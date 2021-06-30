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

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class OpportunitiesOrchestrator : IOpportunitiesOrchestrator
    {
        private const string All = "All";

        private readonly IDateTimeService _dateTimeService;
        private readonly IOpportunitiesService _opportunitiesService;
        private readonly IEncodingService _encodingService;
        private readonly ITagService _tagService;
        private readonly IUserService _userService;

        public OpportunitiesOrchestrator(IDateTimeService dateTimeService, IOpportunitiesService opportunitiesService, ITagService tagService, IUserService userService, IEncodingService encodingService)
        {
            _dateTimeService = dateTimeService;
            _opportunitiesService = opportunitiesService;
            _encodingService = encodingService;
            _tagService = tagService;
            _userService = userService;
        }

        public async Task<DetailViewModel> GetDetailViewModel(string encodedId)
        {
            int id = (int)_encodingService.Decode(encodedId, EncodingType.PledgeId);

            var opportunityDto = await _opportunitiesService.GetOpportunity(id);

            // If null, an opportunity couldn't be found for the encodedId.
            if (opportunityDto == null)
            {
                return null;
            }

            var opportunitySummaryViewModel = await GetOpportunitySummaryViewModel(opportunityDto);

            return new DetailViewModel()
            {
                EmployerName = opportunitySummaryViewModel.EmployerName,
                EncodedPledgeId = opportunitySummaryViewModel.EncodedPledgeId,
                IsNamePublic = opportunitySummaryViewModel.IsNamePublic,
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

        public async Task<OpportunitySummaryViewModel> GetOpportunitySummaryViewModel(OpportunityDto opportunityDto)
        {
            // Pull back the tags, and use the descriptions to build the lists.
            var sectorTags = await _tagService.GetSectors();

            string sectorList = null;
            if (sectorTags.Count == opportunityDto.Sectors.Count())
            {
                sectorList = All;
            }
            else
            {
                var selectedSectorDescriptions = sectorTags
                    .Where(x => opportunityDto.Sectors.Contains(x.TagId))
                    .Select(x => x.Description);

                sectorList = string.Join(", ", selectedSectorDescriptions);
            }

            var jobRoleTags = await _tagService.GetJobRoles();

            string jobRoleList = null;
            if (jobRoleTags.Count == opportunityDto.JobRoles.Count())
            {
                jobRoleList = All;
            }
            else
            {
                var selectedJobRoleDescriptions = jobRoleTags
                    .Where(x => opportunityDto.JobRoles.Contains(x.TagId))
                    .Select(x => x.Description);

                jobRoleList = string.Join(", ", selectedJobRoleDescriptions);
            }

            bool allContainLevel = opportunityDto.Levels.All(x => x.Contains("Level"));

            if (!allContainLevel)
            {
                // Note: Levels are different in how they're displayed here.
                //       It's been requested that only the number is shown here.
                //       If an additional tag is introduced that doesn't follow the
                //       "Level2", "Level3", structure, then this will break, and
                //       something fancier should probably be conisdered.
                throw new DataException("Unexpected level ID format detected in opportunity levels list. See comment above for more information.");
            }

            var levelTags = await _tagService.GetLevels();

            string levelList = null;
            if (levelTags.Count == opportunityDto.Levels.Count())
            {
                levelList = All;
            }
            else
            {
                var levelDescriptions = opportunityDto.Levels
                .Select(x => x.Replace("Level", string.Empty));

                levelList = string.Join(", ", levelDescriptions);
            }

            DateTime date = _dateTimeService.UtcNow;

            var yearDescription = $"{date.ToTaxYear("yyyy")}/{date.AddYears(1).ToTaxYear("yy")}";

            return new OpportunitySummaryViewModel()
            {
                Amount = opportunityDto.Amount,
                EmployerName = opportunityDto.DasAccountName,
                EncodedPledgeId = _encodingService.Encode(opportunityDto.Id, EncodingType.PledgeId),
                IsNamePublic = opportunityDto.IsNamePublic,
                JobRoleList = jobRoleList,
                LevelList = levelList,
                SectorList = sectorList,
                YearDescription = yearDescription,
            };
        }
    }
}