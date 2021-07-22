using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetLevelResponse
    {
        public IEnumerable<ReferenceDataItem> Levels { get; set; }
    }
}
