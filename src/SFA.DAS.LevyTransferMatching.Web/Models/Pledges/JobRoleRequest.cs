using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class JobRoleRequest
    {
        public string EncodedAccountId { get; set; }
        public Guid CacheKey { get; set; }
    }
}
