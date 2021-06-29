using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Shared
{
    public class OpportunitySummaryViewModel
    {
        public OpportunityDetail OpportunityDetail { get; set; }
        public string SectorList { get; set; }
        public string JobRoleList { get; set; }
        public string LevelList { get; set; }
        public string YearDescription { get; set; }
    }
}