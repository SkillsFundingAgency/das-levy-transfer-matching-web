using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class MoreDetailsPostRequest : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }
        [MaxWords(200)]
        public string Details { get; set; }
    }
}
