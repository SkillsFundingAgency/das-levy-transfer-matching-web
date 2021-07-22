namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types
{
    public class GetAmountResponse
    {
        public string DasAccountName { get; set; }
        public decimal RemainingTransferAllowance { get; set; }
    }
}
