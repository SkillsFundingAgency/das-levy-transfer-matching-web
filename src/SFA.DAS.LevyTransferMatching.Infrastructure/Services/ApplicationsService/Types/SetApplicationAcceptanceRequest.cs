namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types
{
    public class SetApplicationAcceptanceRequest
    {
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int ApplicationId { get; set; }
        public long AccountId { get; set; }
        public ApplicationAcceptance Acceptance { get; set; }

        public enum ApplicationAcceptance
        {
            Accept,
            Decline
        }
    }
}