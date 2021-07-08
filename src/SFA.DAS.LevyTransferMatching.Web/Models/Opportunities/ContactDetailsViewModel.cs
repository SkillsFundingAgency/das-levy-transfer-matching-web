using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ContactDetailsViewModel : ContactDetailsPostRequest
    {
        public string DasAccountName { get; set; }
        public OpportunitySummaryViewModel OpportunitySummaryViewModel { get; set; }
    }
}