using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types
{
    public class GetSectorResponse
    {
        public IEnumerable<ReferenceDataItem> Sectors { get; set; }
        public OpportunityDto Opportunity { get; set; }
        public string Location { get; set; }
    }
}