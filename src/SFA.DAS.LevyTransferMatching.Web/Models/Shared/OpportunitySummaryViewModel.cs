namespace SFA.DAS.LevyTransferMatching.Web.Models.Shared
{
    public class OpportunitySummaryViewModel
    {
        public string Description { get; set; }
        public int Amount { get; set; }
        public string SectorList { get; set; }
        public string JobRoleList { get; set; }
        public string LevelList { get; set; }
        public string LocationList { get; set; }
        public bool IsNamePublic { get; set; }
        public bool HideFooter { get; set; }
    }
}