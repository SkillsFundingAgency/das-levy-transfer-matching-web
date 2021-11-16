using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationApprovalOptionsRequest
    {
        public string EncodedAccountId { get; set; }
        public string EncodedPledgeId { get; set; }
        public string EncodedApplicationId { get; set; }
        public string EmployerAccountName { get; set; }
    }
}
