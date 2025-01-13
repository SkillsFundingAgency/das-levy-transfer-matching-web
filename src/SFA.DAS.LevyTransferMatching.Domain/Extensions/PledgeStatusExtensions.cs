using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Domain.Extensions;

public static class PledgeStatusExtensions
{
    public static string GetLabel(this PledgeStatus status)
    {
        return status switch
        {
            PledgeStatus.Active => "Active",
            PledgeStatus.Closed => "Closed",
            _ => string.Empty
        };
    }

    public static string GetCssClass(this PledgeStatus status)
    {
        return status switch
        {
            PledgeStatus.Active => "govuk-tag govuk-tag--dark-blue",
            PledgeStatus.Closed => "govuk-tag govuk-tag--grey",
            _ => string.Empty
        };
    }
}