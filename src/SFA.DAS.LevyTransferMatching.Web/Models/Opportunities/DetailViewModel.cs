using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class DetailViewModel
    {
        public OpportunityDto Opportunity { get; set; }
        public string SectorList { get; internal set; }
        public string JobRoleList { get; internal set; }
        public string LevelList { get; internal set; }
        public string YearDescription { get; internal set; }
    }
}