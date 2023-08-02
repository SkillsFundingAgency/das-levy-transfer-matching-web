using SFA.DAS.LevyTransferMatching.Domain.Types;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class CreatePledgeRequest
    {
        public int Amount { get; set; }
        public bool IsNamePublic { get; set; }
        public string DasAccountName { get; set; }
        public IEnumerable<string> Sectors { get; set; }
        public IEnumerable<string> JobRoles { get; set; }
        public IEnumerable<string> Levels { get; set; }
        public List<string> Locations { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public AutomaticApprovalOption AutomaticApprovalOption { get; set; }
    }
}
