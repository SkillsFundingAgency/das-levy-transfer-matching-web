using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class AutoApprovePostRequest : BasePledgesRequest
    {
        public AutomaticApprovalOption AutomaticApprovalOption { get; set; }

    }
}
