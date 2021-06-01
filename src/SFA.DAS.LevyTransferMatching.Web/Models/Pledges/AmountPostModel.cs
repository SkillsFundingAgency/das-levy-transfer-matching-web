namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class AmountPostModel
    {
        public string EncodedAccountId { get; set; }
        public string Amount { get; set; }
        public bool? IsNamePublic { get; set; }
    }
}