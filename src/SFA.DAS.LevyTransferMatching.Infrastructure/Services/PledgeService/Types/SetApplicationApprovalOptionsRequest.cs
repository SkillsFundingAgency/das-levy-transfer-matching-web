namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class SetApplicationApprovalOptionsRequest
    {
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public bool AutomaticApproval { get; set; }
    }
}
