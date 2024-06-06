namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class PledgesRequest : BasePledgesRequest
    {
        public bool HasMinimumTransferFunds { get; set; } = true;
    }
}
