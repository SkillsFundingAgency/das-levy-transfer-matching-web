using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class PledgesViewModel : PledgesRequest
    {
        public IEnumerable<Pledge> Pledges { get; set; }
        public PagingData Paging { get; set; }
        public bool RenderCreatePledgeButton { get; set; }
        
        public class Pledge
        {
            public string ReferenceNumber { get; set; }
            public int Amount { get; set; }
            public int RemainingAmount { get; set; }
            public int ApplicationCount { get; set; }
            public PledgeStatus Status { get; set; }
        }

        public class PagingData
        {
            public bool ShowPageLinks { get; set; }
            public int TotalPledges { get; set; }
            public int TotalPages { get; set; }
            public int PageSize { get; set; }
            public int Page { get; set; }
            public IEnumerable<PledgesViewModel.PageLink> PageLinks { get; set; }
            public int PageStartRow { get; set; }
            public int PageEndRow { get; set; }
        }

        public class PageLink
        {
            public string Label { get; set; }
            public string AriaLabel { get; set; }
            public bool? IsCurrent { get; set; }
            public Dictionary<string, string> RouteData { get; set; }
        }
    }
}
