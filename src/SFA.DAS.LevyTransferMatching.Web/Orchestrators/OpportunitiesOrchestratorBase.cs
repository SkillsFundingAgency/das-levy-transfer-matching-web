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

        public OpportunitySummaryViewModel GetOpportunitySummaryViewModel(
            IEnumerable<string> sectors,
            IEnumerable<string> jobRoles,
            IEnumerable<string> levels,
            IEnumerable<string> locations,
            IEnumerable<ReferenceDataItem> allSectors,
            IEnumerable<ReferenceDataItem> allJobRoles,
            IEnumerable<ReferenceDataItem> allLevels,
            int amount,
            bool isNamePublic,
            string dasAccountName,
            string encodedPledgeId)
        {
            string sectorList = sectors.ToReferenceDataDescriptionList(allSectors);
            string jobRoleList = jobRoles.ToReferenceDataDescriptionList(allJobRoles);
            string levelList = levels.ToReferenceDataDescriptionList(allLevels, descriptionSource: x => x.ShortDescription);
            string locationList = locations.ToLocationsList();

            DateTime dateTime = _dateTimeService.UtcNow;

            return new OpportunitySummaryViewModel()
            {
                Amount = amount,
                Description = isNamePublic ? $"{dasAccountName} ({encodedPledgeId})" : "A levy-paying business",
                JobRoleList = jobRoleList,
                LevelList = levelList,
                SectorList = sectorList,
                LocationList = locationList,
                YearDescription = dateTime.ToTaxYearDescription(),
                IsNamePublic = isNamePublic
            };
        }
    }
}