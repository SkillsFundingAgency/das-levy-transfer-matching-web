using SFA.DAS.LevyTransferMatching.Domain.Types;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Cache
{
    public class CreatePledgeCacheItem
    {
        public CreatePledgeCacheItem(Guid key)
        {
            Key = key;
            AutomaticApprovalOption = AutomaticApprovalOption.NotApplicable;
        }

        public Guid Key { get; set; }

        public int? Amount { get; set; }
        public bool? IsNamePublic { get; set; }
        public string DasAccountName { get; set; }
        public AutomaticApprovalOption AutomaticApprovalOption { get; set; }
        public List<string> Sectors { get; set; }
        public List<string> JobRoles { get; set; }
        public List<string> Levels { get; set; }
        public List<string> Locations { get; set; }
    }
}