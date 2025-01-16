using static SFA.DAS.LevyTransferMatching.Web.Models.Opportunities.IndexViewModel;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class IndexRequest
    {
        public const int DefaultPageSize = 30;
        public IEnumerable<string> Sectors { get; set; }
        public int? Page { get; set; }
        public string SortBy { get; set; }
    }
}