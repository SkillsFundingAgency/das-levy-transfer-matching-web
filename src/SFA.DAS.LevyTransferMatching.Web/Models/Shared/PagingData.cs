namespace SFA.DAS.LevyTransferMatching.Web.Models.Shared;

public class PagingData
{
    public bool ShowPageLinks { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int Page { get; set; }
    public IEnumerable<PageLink> PageLinks { get; set; }
    public int PageStartRow { get; set; }
    public int PageEndRow { get; set; }
}
