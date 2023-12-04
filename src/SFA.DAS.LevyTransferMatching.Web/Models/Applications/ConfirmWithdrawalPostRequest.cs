namespace SFA.DAS.LevyTransferMatching.Web.Models.Applications;

public class ConfirmWithdrawalPostRequest : WithdrawalConfirmationRequest
{
    public bool? HasConfirmed { get; set; }
}