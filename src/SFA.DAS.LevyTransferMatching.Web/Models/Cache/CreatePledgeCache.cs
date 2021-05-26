using System;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Cache
{
    public class CreatePledgeCache
    {
        public CreatePledgeCache()
        {
            Key = Guid.NewGuid();
        }

        public Guid Key { get; set; }

        public int? Amount { get; set; }
        public bool? IsNamePublic { get; set; }
    }
}
