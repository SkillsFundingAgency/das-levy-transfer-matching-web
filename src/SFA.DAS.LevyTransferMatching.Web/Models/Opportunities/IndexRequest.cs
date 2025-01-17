using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class IndexRequest
    {
        public const int DefaultPageSize = 30;
        public IEnumerable<string> Sectors { get; set; }
        public string CommaSeparatedSectors { get; set; }
        public int? Page { get; set; }
        public string SortBy { get; set; } = OpportunitiesSortBy.ValueHighToLow;

        public IEnumerable<string> GetSectorsList()
        {
            return string.IsNullOrWhiteSpace(CommaSeparatedSectors) ? Sectors : CommaSeparatedSectors.Split(',');
        }
    }
}