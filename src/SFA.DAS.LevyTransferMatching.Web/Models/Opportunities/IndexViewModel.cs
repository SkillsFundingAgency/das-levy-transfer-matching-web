using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using System.Globalization;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class IndexViewModel
    {
        public List<Opportunity> Opportunities { get; set; }
        public PagingData Paging { get; set; }
        public List<ReferenceDataItem> Sectors { get; set; }
        public bool isSectorFilterApplied { get; set; }

        public class Opportunity
        {
            public int Amount { get; set; }
            public string EmployerName { get; set; }
            public string ReferenceNumber { get; set; }
            public string Locations { get; set; }
            public string Sectors { get; set; }
            public string JobRoles { get; set; }
            public string Levels { get; set; }
            public string DisplayAmount => Amount.ToString("C0", new CultureInfo("en-GB"));
        }

        public class PagingData
        {
            public bool ShowPageLinks { get; set; }
            public int TotalOpportunities { get; set; }
            public int TotalPages { get; set; }
            public int PageSize { get; set; }
            public int Page { get; set; }
            public IEnumerable<IndexViewModel.PageLink> PageLinks { get; set; }
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
