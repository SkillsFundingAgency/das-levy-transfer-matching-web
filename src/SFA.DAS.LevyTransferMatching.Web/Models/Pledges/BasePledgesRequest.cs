using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public abstract class BasePledgesRequest
    {
        public string EncodedAccountId { get; set; }
        public Guid CacheKey { get; set; }

        [AutoDecode(nameof(EncodedAccountId), EncodingType.AccountId)]
        public long AccountId { get; set; }
    }
}