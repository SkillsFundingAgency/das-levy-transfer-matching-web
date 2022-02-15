using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class RejectApplicationsPostRequest
    {
        public string EncodedAccountId { get; set; }

        [AutoDecode(nameof(EncodedAccountId), EncodingType.AccountId)]
        public int AccountId { get; set; }

        public string EncodedPledgeId { get; set; }

        [AutoDecode(nameof(EncodedPledgeId), EncodingType.PledgeId)]
        public int PledgeId { get; set; }

        public List<int> ApplicationsToReject { get; set; }

        public bool RejectConfirm { get; set; }
    }
}