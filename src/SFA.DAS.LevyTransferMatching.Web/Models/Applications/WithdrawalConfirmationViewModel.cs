namespace SFA.DAS.LevyTransferMatching.Web.Models.Applications
{
    public class WithdrawalConfirmationViewModel : ConfirmWithdrawalPostRequest
    {
        public string PledgeEmployerName { get; set; }
        public string EncodedPledgeId { get; set; }
    }
}
