namespace SFA.DAS.LevyTransferMatching.Domain.Types;

public class PagedModel
{
    public int TotalResults { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int Page { get; set; }
}
