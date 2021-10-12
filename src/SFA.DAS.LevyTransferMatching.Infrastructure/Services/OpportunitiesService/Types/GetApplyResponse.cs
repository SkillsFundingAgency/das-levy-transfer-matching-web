using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types
{
    public class GetApplyResponse
    {
        public OpportunityData Opportunity { get; set; }
        public IEnumerable<ReferenceDataItem> Sectors { get; set; }
        public IEnumerable<ReferenceDataItem> JobRoles { get; set; }
        public IEnumerable<ReferenceDataItem> Levels { get; set; }

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

        public IEnumerable<PledgeLocation> PledgeLocations { get; set; }
        public class PledgeLocation
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
