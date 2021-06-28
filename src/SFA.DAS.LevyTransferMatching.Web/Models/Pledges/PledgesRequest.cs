using System;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public abstract class PledgesRequest
    {
        public string EncodedAccountId { get; set; }
        public Guid CacheKey { get; set; }

        [AutoDecode("EncodedAccountId", EncodingType.AccountId)]
        public long AccountId { get; set; }
    }
}