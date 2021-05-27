using System;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Enums;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class CreateViewModel
    {
        public string EncodedAccountId { get; set; }
        public Guid CacheKey { get; set; }
        public int? Amount { get; set; }
        public bool? IsNamePublic { get; set; }
        public Sector Sectors { get; set; }

        public string IsNamePublicDisplayValue => IsNamePublic.HasValue ? IsNamePublic.Value ? "Show" : "Hide" : "-";
        public bool AreAllSectorsSelected => Sectors == Sector.None || Sectors == EnumExtensions.GetMaxValue<Sector>();
    }
}