using System;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Cache
{
    public class CreatePledgeCacheItem
    {
        public CreatePledgeCacheItem()
        {
            Key = Guid.NewGuid();
            Levels = new List<string>();
        }

        public CreatePledgeCacheItem(Guid key)
        {
            Key = key;
        }

        public Guid Key { get; set; }

        public int? Amount { get; set; }
        public bool? IsNamePublic { get; set; }
        public string DasAccountName { get; set; }
        public List<string> Sectors { get; set; }
        public List<string> JobRoles { get; set; }
        public List<string> Levels { get; set; }
    }
}
