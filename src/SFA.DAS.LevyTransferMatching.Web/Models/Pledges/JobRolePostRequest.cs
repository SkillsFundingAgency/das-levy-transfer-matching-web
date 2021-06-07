using SFA.DAS.LevyTransferMatching.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class JobRolePostRequest
    {
        public string EncodedAccountId { get; set; }
        public Guid CacheKey { get; set; }
        public JobRole? JobRoles { get; set; }
    }
}
