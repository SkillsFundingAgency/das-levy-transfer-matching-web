using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types
{
    public class GetIndexResponse
    {
        public List<OpportunityDto> Opportunities { get; set; }
        public List<ReferenceDataItem> Levels { get; set; }
        public List<ReferenceDataItem> Sectors { get; set; }
        public List<ReferenceDataItem> JobRoles { get; set; }
    }
}
