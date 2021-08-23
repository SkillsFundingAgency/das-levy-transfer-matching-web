namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class AmountPostRequest : BasePledgesRequest
    {
        public string Amount { get; set; }
        public string RemainingTransferAllowance { get; set; }
        public bool? IsNamePublic { get; set; }
        public string DasAccountName { get; set; }
    }
}