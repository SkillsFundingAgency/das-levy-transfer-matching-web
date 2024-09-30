namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class IndexRequest
    {
        public const int DefaultPageSize = 50;
        public IEnumerable<string> Sectors { get; set; }
        public int? Page { get; set; }
    }
}