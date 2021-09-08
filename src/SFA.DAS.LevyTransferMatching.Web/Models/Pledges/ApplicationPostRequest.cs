using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationPostRequest
    {
        [AutoDecode(nameof(EncodedApplicationId), EncodingType.PledgeApplicationId)]
        public int ApplicationId { get; set; }

        public string EncodedApplicationId { get; set; }

        public ApprovalAction? SelectedAction { get; set; }

        public enum ApprovalAction
        {
            Approve,
            Reject
        }
    }
}