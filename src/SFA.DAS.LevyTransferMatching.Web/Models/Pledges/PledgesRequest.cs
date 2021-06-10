using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public abstract class PledgesRequest
    {
        public string EncodedAccountId { get; set; }
        public Guid CacheKey { get; set; }
    }
}