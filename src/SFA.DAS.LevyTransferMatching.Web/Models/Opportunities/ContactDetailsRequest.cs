using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ContactDetailsRequest
    {
        public string EncodedAccountId { get; set; }
        public string EncodedPledgeId { get; set; }
        public Guid CacheKey { get; set; }
    }
}