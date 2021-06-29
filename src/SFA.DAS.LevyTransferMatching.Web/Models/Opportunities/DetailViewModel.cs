using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class DetailViewModel : OpportunitiesPostRequest
    {
        public Opportunity Opportunity { get; set; }

        public OpportunitySummaryViewModel OpportunitySummaryView { get; set; }
    }
}