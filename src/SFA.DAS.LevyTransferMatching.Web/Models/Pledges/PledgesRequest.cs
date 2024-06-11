namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class PledgesRequest : BasePledgesRequest
    {
        public const int DefaultPageSize = 50;
        [FromQuery]
        public int? Page { get; set; } 
    }
}
