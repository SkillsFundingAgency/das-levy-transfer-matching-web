using System;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public abstract class OpportunitiesOrchestratorBase
    {
        private readonly IDateTimeService _dateTimeService;

        protected OpportunitiesOrchestratorBase(IDateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;
        }

        public OpportunitySummaryViewModel GetOpportunitySummaryViewModel(GetOpportunitySummaryViewModelOptions options)
        {
            var sectorList = options.Sectors.ToReferenceDataDescriptionList(options.AllSectors);
            var jobRoleList = options.JobRoles.ToReferenceDataDescriptionList(options.AllJobRoles);
            var levelList = options.Levels.ToReferenceDataDescriptionList(options.AllLevels, descriptionSource: x => x.ShortDescription);
            var locationList = options.Locations.ToLocationsList();

            DateTime dateTime = _dateTimeService.UtcNow;

            return new OpportunitySummaryViewModel()
            {
                Amount = options.Amount,
                Description = options.IsNamePublic ? $"{options.DasAccountName} ({options.EncodedPledgeId})" : "A levy-paying business",
                JobRoleList = jobRoleList,
                LevelList = levelList,
                SectorList = sectorList,
                LocationList = locationList,
                YearDescription = dateTime.ToTaxYearDescription(),
                IsNamePublic = options.IsNamePublic,
            };
        }

        public class GetOpportunitySummaryViewModelOptions
        {
            public IEnumerable<string> Sectors { get; set; }
            public IEnumerable<string> JobRoles { get; set; }
            public IEnumerable<string> Levels { get; set; }
            public IEnumerable<string> Locations { get; set; }
            public IEnumerable<ReferenceDataItem> AllSectors { get; set; }
            public IEnumerable<ReferenceDataItem> AllJobRoles { get; set; }
            public IEnumerable<ReferenceDataItem> AllLevels { get; set; }
            public int Amount { get; set; }
            public bool IsNamePublic { get; set; }
            public string DasAccountName { get; set; }
            public string EncodedPledgeId { get; set; }
        }
    }
}