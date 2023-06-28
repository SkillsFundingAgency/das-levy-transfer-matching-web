namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class AmountPostRequest : BasePledgesRequest
    {
        public string Amount { get; set; }
        public string RemainingTransferAllowance { get; set; }
    }
}