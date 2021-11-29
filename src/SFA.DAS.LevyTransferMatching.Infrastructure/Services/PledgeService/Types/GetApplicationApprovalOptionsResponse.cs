using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetApplicationApprovalOptionsResponse
    {
        public string EmployerAccountName { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
    }
}
