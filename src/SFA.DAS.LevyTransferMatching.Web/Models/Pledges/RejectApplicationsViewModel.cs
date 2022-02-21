using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class RejectApplicationsViewModel : RejectApplicationsPostRequest
    {
        public List<string> DasAccountNames { get; set; }
        public RejectApplicationsViewModel()
        {
            DasAccountNames = new List<string>();            
        }
    }
}
