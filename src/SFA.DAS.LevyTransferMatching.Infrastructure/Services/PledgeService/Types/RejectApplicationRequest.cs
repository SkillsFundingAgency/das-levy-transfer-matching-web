using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class RejectApplicationRequest
    {
        public List<string> ApplicationsToReject { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
    }
}
