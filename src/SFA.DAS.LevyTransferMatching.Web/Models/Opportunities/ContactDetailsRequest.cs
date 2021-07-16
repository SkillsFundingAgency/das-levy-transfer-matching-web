using SFA.DAS.LevyTransferMatching.Web.Attributes;
using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ContactDetailsRequest
    {
        public string EncodedAccountId { get; set; }
        [AutoDecode(nameof(EncodedAccountId), Encoding.EncodingType.AccountId)]
        public long AccountId { get; set; }
        public string EncodedPledgeId { get; set; }
        [AutoDecode(nameof(EncodedPledgeId), Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }
        public Guid CacheKey { get; set; }
    }
}