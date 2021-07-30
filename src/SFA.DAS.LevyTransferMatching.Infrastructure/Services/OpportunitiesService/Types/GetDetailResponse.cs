using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types
{
    public class GetDetailResponse
    {
        public Opportunity OpportunitySummary { get; set; }
        public List<ReferenceDataItem> Sectors { get; set; }
        public List<ReferenceDataItem> JobRoles { get; set; }
        public List<ReferenceDataItem> Levels { get; set; }

        public class Opportunity
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
    }
}
