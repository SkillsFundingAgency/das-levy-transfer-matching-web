using System;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Cache
{
    public class CreateApplicationCacheItem
    {
        public CreateApplicationCacheItem()
        {
            Key = Guid.NewGuid();
        }

        public CreateApplicationCacheItem(Guid key)
        {
            Key = key;
        }

        public Guid Key { get; set; }

        public string Details { get; set; }
    }
}
