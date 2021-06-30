using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class DetailViewModel : DetailPostRequest
    {
        public string EmployerName { get; set; }
        public bool IsNamePublic { get; set; }

        public OpportunitySummaryViewModel OpportunitySummaryView { get; set; }
    }
}