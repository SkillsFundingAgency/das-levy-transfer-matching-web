using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class SelectAccountRequest
    {
        public string EncodedOpportunityId { get; set; }
        [AutoDecode(nameof(EncodedOpportunityId), Encoding.EncodingType.PledgeId)]
        public int OpportunityId { get; set; }
    }
}