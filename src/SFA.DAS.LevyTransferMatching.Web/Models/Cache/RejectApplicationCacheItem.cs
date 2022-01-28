using System;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Cache
{
    public class RejectApplicationCacheItem
    {
        public RejectApplicationCacheItem(Guid key)
        {
            Key = key;
        }

        public Guid Key { get; set; }

        public List<string> ApplicationsToReject { get; set; }
    }
}