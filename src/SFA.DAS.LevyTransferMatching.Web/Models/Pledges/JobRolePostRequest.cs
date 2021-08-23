using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class JobRolePostRequest : BasePledgesRequest
    {
        public List<string> JobRoles { get; set; }
    }
}
