﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public JobRole? JobRoles { get; set; }
        public List<string> Locations { get; set; }

        public string IsNamePublicDisplayValue => IsNamePublic.HasValue ? IsNamePublic.Value ? "Show" : "Hide" : "-";
        public bool AreAllSectorsSelected => !Sectors.HasValue || Sectors == Sector.None || Sectors == EnumExtensions.GetMaxValue<Sector>();
        public bool AreAllJobRolesSelected => !JobRoles.HasValue || JobRoles == JobRole.None || JobRoles == EnumExtensions.GetMaxValue<JobRole>();
        public bool AreAllLevelsSelected => !Levels.HasValue || Levels == Level.None || Levels == EnumExtensions.GetMaxValue<Level>();
        public bool AreAllLocationsSelected => Locations == null || Locations.Count == 0 || Locations.All(x => x == null);
    }
}