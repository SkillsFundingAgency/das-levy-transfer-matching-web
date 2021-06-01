using System;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Enums;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class CreateViewModel : CreateRequest
    {
        public int? Amount { get; set; }
        public bool? IsNamePublic { get; set; }
        public Sector? Sectors { get; set; }
        public Level? Levels { get; set; }

        public string IsNamePublicDisplayValue => IsNamePublic.HasValue ? IsNamePublic.Value ? "Show" : "Hide" : "-";
        public bool AreAllSectorsSelected => !Sectors.HasValue || Sectors == Sector.None || Sectors == EnumExtensions.GetMaxValue<Sector>();
        public bool AreAllLevelsSelected => !Levels.HasValue || Levels == Level.None || Levels == EnumExtensions.GetMaxValue<Level>();
    }
}