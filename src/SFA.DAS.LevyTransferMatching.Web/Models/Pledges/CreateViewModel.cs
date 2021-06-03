using System;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Enums;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class CreateViewModel : CreatePostRequest
    {
        public string IsNamePublicDisplayValue => IsNamePublic.HasValue ? IsNamePublic.Value ? "Show" : "Hide" : "-";
        public bool AreAllSectorsSelected => !Sectors.HasValue || Sectors == Sector.None || Sectors == EnumExtensions.GetMaxValue<Sector>();
        public bool AreAllJobRolesSelected => !JobRoles.HasValue || JobRoles == JobRole.None || JobRoles == EnumExtensions.GetMaxValue<JobRole>();
        public bool AreAllLevelsSelected => !Levels.HasValue || Levels == Level.None || Levels == EnumExtensions.GetMaxValue<Level>();
        public bool AmountSectionComplete => Amount.HasValue && IsNamePublic.HasValue;
    }
}