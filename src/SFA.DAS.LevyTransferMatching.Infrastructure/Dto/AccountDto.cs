namespace SFA.DAS.LevyTransferMatching.Infrastructure.Dto;

public class AccountDto
{
    public string EncodedAccountId { get; set; }
    public string DasAccountName { get; set; }
    public decimal RemainingTransferAllowance { get; set; }
}