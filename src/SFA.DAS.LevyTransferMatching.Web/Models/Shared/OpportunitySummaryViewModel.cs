using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Shared
{
    public class OpportunitySummaryViewModel
    {
        public string EncodedPledgeId { get; set; }
        public string EmployerName { get; set; }
        public bool IsNamePublic { get; set; }
        public int Amount { get; set; }
        public string SectorList { get; set; }
        public string JobRoleList { get; set; }
        public string LevelList { get; set; }
        public string YearDescription { get; set; }
    }
}