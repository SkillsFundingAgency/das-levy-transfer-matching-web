using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Domain.Extensions
{
    public static class PledgeStatusExtenstions
    {
        public static string GetLabel(this PledgeStatus status)
        {
            switch (status)
            {
                case PledgeStatus.Active: return "Active";
                case PledgeStatus.Closed: return "Closed";
                default:
                    return string.Empty;
            }
        }

        public static string GetCssClass(this PledgeStatus status)
        {
            switch (status)
            {
                case PledgeStatus.Active: return "govuk-tag govuk-tag--dark-blue";
                case PledgeStatus.Closed: return "govuk-tag govuk-tag--grey";
                default:
                    return string.Empty;
            }
        }
    }
}
