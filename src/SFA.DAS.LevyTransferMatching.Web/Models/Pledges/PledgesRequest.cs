namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class PledgesRequest : BasePledgesRequest
    {
        public bool PledgeClosedShowBanner { get; set; }
        public string PledgeClosedEncodedPledgeId { get; set; }
    }
}
