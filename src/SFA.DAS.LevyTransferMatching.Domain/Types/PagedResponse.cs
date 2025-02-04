using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Domain.Types;

public class PagedResponse<T>
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int Page { get; set; }
    public IEnumerable<T> Items { get; set; }
}