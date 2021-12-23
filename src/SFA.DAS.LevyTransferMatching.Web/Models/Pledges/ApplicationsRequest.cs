using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationsRequest
    {
        public string EncodedAccountId { get; set; }
        [AutoDecode(nameof(EncodedAccountId), EncodingType.AccountId)]
        public int AccountId { get; set; }
        public string EncodedPledgeId { get; set; }
        [AutoDecode("EncodedPledgeId", Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }
        public bool DisplayRejectedBanner { get; set; }
        public string RejectedEmployerName { get; set; }
    }
}
