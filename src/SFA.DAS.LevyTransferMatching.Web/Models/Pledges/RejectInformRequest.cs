using SFA.DAS.LevyTransferMatching.Web.Attributes;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class RejectInformRequest : PledgesRequest
    {
        public string EncodedPledgeId { get; set; }

        [AutoDecode("EncodedPledgeId", Encoding.EncodingType.PledgeId)]
        public int PledgeId { get; set; }

        public List<string> ApplicationsToReject { get; set; }

        public bool HasApplicationsToReject { get; set; }
    }
}