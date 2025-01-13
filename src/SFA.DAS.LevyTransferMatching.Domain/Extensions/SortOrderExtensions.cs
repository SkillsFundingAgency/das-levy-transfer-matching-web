using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Domain.Extensions;

public static class SortOrderExtensions
{
    public static SortOrder Reverse(this SortOrder sortOrder)
    {
        return sortOrder == SortOrder.Ascending
            ? SortOrder.Descending
            : SortOrder.Ascending;
    }
}