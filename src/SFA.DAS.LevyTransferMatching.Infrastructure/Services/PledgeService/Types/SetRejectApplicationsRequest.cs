using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class SetRejectApplicationsRequest
    {
        public List<int> ApplicationsToReject { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public SetRejectApplicationsRequest()
        {
            ApplicationsToReject = new List<int>();
        }
    }
}
