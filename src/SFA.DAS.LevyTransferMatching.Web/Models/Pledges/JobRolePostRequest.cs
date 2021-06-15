using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class JobRolePostRequest : PledgesRequest
    {
        public List<string> JobRoles { get; set; }
    }
}
