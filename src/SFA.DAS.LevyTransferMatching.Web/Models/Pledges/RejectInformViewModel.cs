using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class RejectInformViewModel : BasePledgesRequest
    {
        public List<string> ApplicationsToReject { get; set; }
    }
}
