using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Cache
{
    public class CreatePledgeCacheItem
    {
        public CreatePledgeCacheItem()
        {
            Key = Guid.NewGuid();
        }

        public CreatePledgeCacheItem(Guid key)
        {
            Key = key;
        }

        public Guid Key { get; set; }

        public int? Amount { get; set; }
        public bool? IsNamePublic { get; set; }
    }
}
