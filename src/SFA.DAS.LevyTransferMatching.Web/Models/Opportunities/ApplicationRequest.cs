using SFA.DAS.LevyTransferMatching.Web.Attributes;
using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ApplicationRequest : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }
        [AutoDecode(nameof(EncodedPledgeId), Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }

        [AutoDecode(nameof(EncodedAccountId), Encoding.EncodingType.AccountId)]
        public long AccountId { get; set; }
        public bool AccessToMultipleAccounts { get; set; }
    }
}
