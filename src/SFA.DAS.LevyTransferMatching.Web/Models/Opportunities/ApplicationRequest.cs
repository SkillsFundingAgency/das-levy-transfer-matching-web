using SFA.DAS.LevyTransferMatching.Web.Attributes;
using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ApplicationRequest
    {
        public string EncodedPledgeId { get; set; }
        [AutoDecode(nameof(EncodedPledgeId), Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }
        public string EncodedAccountId { get; set; }
        public Guid CacheKey { get; set; }
    }
}
