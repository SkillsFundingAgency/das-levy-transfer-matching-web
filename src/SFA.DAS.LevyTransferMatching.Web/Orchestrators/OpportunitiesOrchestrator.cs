using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class OpportunitiesOrchestrator : IOpportunitiesOrchestrator
    {
        private const string All = "All";

        private readonly IDateTimeService _dateTimeService;
        private readonly IOpportunitiesService _opportunitiesService;
        private readonly ITagService _tagService;

        public OpportunitiesOrchestrator(IDateTimeService dateTimeService, IOpportunitiesService opportunitiesService, ITagService tagService)
        {
            _dateTimeService = dateTimeService;
            _opportunitiesService = opportunitiesService;
            _tagService = tagService;
        }

        public async Task<DetailViewModel> GetDetailViewModel(string encodedId)
        {
            var opportunityDto = await _opportunitiesService.GetOpportunity(encodedId);

            // If null, an opportunity couldn't be found for the encodedId.
            if (opportunityDto == null)
            {
                return null;
            }

            // Pull back the tags, and use the descriptions to build the lists.
            var sectorTags = await _tagService.GetSectors();

            string sectorList = null;
            if (sectorTags.Count() == opportunityDto.Sectors.Count())
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
            if (jobRoleTags.Count() == opportunityDto.JobRoles.Count())
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
            if (levelTags.Count() == opportunityDto.Levels.Count())
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

            return new DetailViewModel()
            {
                Opportunity = opportunityDto,
                SectorList = sectorList,
                JobRoleList = jobRoleList,
                LevelList = levelList,
                YearDescription = yearDescription,
            };
        }

        public async Task<IndexViewModel> GetIndexViewModel()
        {
            var opportunitiesDto = await _opportunitiesService.GetAllOpportunities();
            List<Opportunity> opportunities = opportunitiesDto.Select(x => new Opportunity { EmployerName = x.DasAccountName, ReferenceNumber = x.EncodedPledgeId }).ToList();

            return new IndexViewModel { Opportunities = opportunities };
        }
    }
}
