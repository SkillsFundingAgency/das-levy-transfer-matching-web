using System.Collections.Generic;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class JobRoleViewModel : JobRolePostRequest
    {
        public List<ReferenceDataItem> JobRoleOptions { get; set; }
        public bool AreAllJobRolesSelected => JobRoles == null || !JobRoles.Any() || JobRoles.Count == JobRoleOptions.Count;
    }
}
