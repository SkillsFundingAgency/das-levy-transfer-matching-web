using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class DetailRequest
    {
        public string EncodedPledgeId { get; set; }

        [AutoDecode(nameof(EncodedPledgeId), Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }
    }
}