using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationApprovedRequest
    {
        public string EncodedAccountId { get; set; }
        public string EncodedPledgeId { get; set; }
        public string EncodedApplicationId { get; set; }

        [AutoDecode(nameof(EncodedAccountId), EncodingType.AccountId)]
        public long AccountId { get; set; }

        [AutoDecode(nameof(EncodedPledgeId), EncodingType.PledgeId)]
        public int PledgeId { get; set; }

        //[AutoDecode(nameof(EncodedApplicationId), EncodingType.ApplicationId)]
        public int ApplicationId { get; set; }
    }
}
