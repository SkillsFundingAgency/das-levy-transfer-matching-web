using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types
{
    public class GetSectorResponse
    {
        public OpportunityData Opportunity { get; set; }
        public IEnumerable<PledgeLocation> PledgeLocations { get; set; }
        public string Location { get; set; }
        public List<ReferenceDataItem> Sectors { get; set; }
        public List<ReferenceDataItem> JobRoles { get; set; }
        public List<ReferenceDataItem> Levels { get; set; }

        public class OpportunityData
        {
            public int Id { get; set; }
            public string DasAccountName { get; set; }
            public IEnumerable<string> JobRoles { get; set; }
            public IEnumerable<string> Levels { get; set; }
            public IEnumerable<string> Sectors { get; set; }
            public IEnumerable<string> Locations { get; set; }
            public int Amount { get; set; }
            public bool IsNamePublic { get; set; }
        }

        public class PledgeLocation
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}