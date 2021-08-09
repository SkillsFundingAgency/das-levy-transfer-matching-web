using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class MoreDetailsRequest : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }
        [AutoDecode("EncodedPledgeId", Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }
        [AutoDecode("EncodedAccountId", Encoding.EncodingType.AccountId)]
        public int AccountId { get; set; }
    }
}
