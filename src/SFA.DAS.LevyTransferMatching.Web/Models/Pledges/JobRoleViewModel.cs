using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class JobRoleViewModel : JobRolePostRequest
    {
        public List<ReferenceDataItem> JobRoleOptions { get; set; }
    }
}
