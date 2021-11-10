namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationApprovedViewModel
    {
        public string EncodedAccountId { get; set; }
        public string EncodedPledgeId { get; set; }
        public string DasAccountName { get; set; }
        public bool WasAutoApproved { get; set; }
    }
}
