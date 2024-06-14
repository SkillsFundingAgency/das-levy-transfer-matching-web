namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class IndexRequest
    {
        public const int DefaultPageSize = 5;
        public IEnumerable<string> Sectors { get; set; }
        public int? Page { get; set; }
    }
}