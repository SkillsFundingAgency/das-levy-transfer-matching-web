using System;
using SFA.DAS.LevyTransferMatching.Web.Models.Cache;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class CreateViewModel
    {
        public string EncodedAccountId { get; set; }
        public Guid CacheKey { get; set; }
        public int? Amount { get; set; }
        public bool? IsNamePublic { get; set; }

        public string IsNamePublicDisplayValue => IsNamePublic.HasValue ? IsNamePublic.Value ? "Show" : "Hide" : "-";
    }
}