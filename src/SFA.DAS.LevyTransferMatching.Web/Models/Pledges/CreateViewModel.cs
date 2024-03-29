﻿using System.Collections.Generic;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class CreateViewModel : CreateRequest
    {
        public int? Amount { get; set; }
        public bool? IsNamePublic { get; set; }

        public List<string> Sectors { get; set; }
        public List<string> JobRoles { get; set; }
        public List<string> Levels { get; set; }
        public bool AutoApprovalIsEnabled { get; set; }
        public AutomaticApprovalOption AutomaticApprovalOption { get; set; }
        
        public List<ReferenceDataItem> LevelOptions { get; set; }
        public List<ReferenceDataItem> SectorOptions { get; set; }
        public List<ReferenceDataItem> JobRoleOptions { get; set; }
        public List<string> Locations { get; set; }

        public string IsNamePublicDisplayValue => IsNamePublic.HasValue ? IsNamePublic.Value ? "Show" : "Hide" : "-";
        public bool AreAllSectorsSelected => Sectors == null || !Sectors.Any() || Sectors.Count == SectorOptions.Count;
        public bool AreAllJobRolesSelected => JobRoles == null || !JobRoles.Any() || JobRoles.Count == JobRoleOptions.Count;
        public bool AreAllLevelsSelected => Levels == null || !Levels.Any() || Levels.Count == LevelOptions.Count;
        public bool AmountSectionComplete => Amount.HasValue;
        public bool AutoApproveSectionComplete => AutomaticApprovalOption != AutomaticApprovalOption.NotApplicable;
        public bool OrganisationNameSectionComplete => IsNamePublic.HasValue;

        public bool AreAllLocationsSelected => Locations == null || Locations.Count == 0 || Locations.All(x => x == null);

        public bool MandatorySectionsAreComplete => AmountSectionComplete && OrganisationNameSectionComplete && (!AutoApprovalIsEnabled || AutoApproveSectionComplete);
    }
}