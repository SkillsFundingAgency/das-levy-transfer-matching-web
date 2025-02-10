namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

public class IndexRequest
{
    public const int DefaultPageSize = 30;
    public IEnumerable<string> Sectors { get; set; }
    public string CommaSeparatedSectors { get; set; }
    public int? Page { get; set; }
    public string SortBy { get; set; }

    public IEnumerable<string> GetSectorsList()
    {
        return string.IsNullOrWhiteSpace(CommaSeparatedSectors) ? Sectors : CommaSeparatedSectors.Split(',');
    }

    public string PopulateCommaSeparatedSectorsFromSectors()
    {
       return string.Join(",", Sectors.Select(Uri.EscapeDataString));
    }
}