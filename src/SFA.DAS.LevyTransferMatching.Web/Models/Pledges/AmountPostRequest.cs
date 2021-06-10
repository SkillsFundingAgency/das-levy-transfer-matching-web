namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class AmountPostRequest : PledgesRequest
    {
        public string Amount { get; set; }
        public string RemainingTransferAllowance { get; set; }
        public bool? IsNamePublic { get; set; }
    }
}