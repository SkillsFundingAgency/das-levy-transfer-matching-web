using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetJobRoleResponse
    {
        public IEnumerable<ReferenceDataItem> JobRoles { get; set; }
        public IEnumerable<ReferenceDataItem> Sectors { get; set; }
    }
}
