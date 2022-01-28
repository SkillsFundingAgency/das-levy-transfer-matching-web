using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class RejectApplicationPostRequest
    {
        // TODO remove unused fields at the end
        public string EncodedAccountId { get; set; }

        [AutoDecode(nameof(EncodedAccountId), EncodingType.AccountId)]
        public long AccountId { get; set; }

        public string EncodedPledgeId { get; set; }

        [AutoDecode(nameof(EncodedPledgeId), EncodingType.PledgeId)]
        public int PledgeId { get; set; }

        public List<string> ApplicationsToReject { get; set; }

        //public List<Application> ApplicationsToReject { get; set; } // TODO use this instead of above. But before that separate the id from the name. Currently the value is like "VG6RT - Mega Corp"

        public bool RejectConfirm { get; set; }
    }

    public class Application
    {
        public string EncodedApplicationId { get; set; }

        [AutoDecode(nameof(EncodedApplicationId), EncodingType.PledgeApplicationId)]
        public int ApplicationID { get; set; }
    }
}