using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class RejectApplicationsViewModel
    {
        public List<int> ApplicationsToReject { get; set; }
        public string EncodedAccountId { get; set; }
        public string EncodedPledgeId { get; set; }
        public List<string> DasAccountNames { get; set;}

        public RejectApplicationsViewModel()
        {
            DasAccountNames = new List<string>();
        }

    }
}
