using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ApplicationRequest
    {
        public string EncodedAccountId { get; set; }
        public string EncodedPledgeId { get; set; }
        [AutoDecode(nameof(EncodedPledgeId), Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }
    }
}
