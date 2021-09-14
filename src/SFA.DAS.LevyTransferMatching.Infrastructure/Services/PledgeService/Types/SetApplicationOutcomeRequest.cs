namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class SetApplicationOutcomeRequest
    {
        public ApplicationOutcome Outcome { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }

        public enum ApplicationOutcome
        {
            Approve,
            Reject
        }
    }
}
