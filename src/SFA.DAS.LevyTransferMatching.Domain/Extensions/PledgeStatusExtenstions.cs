using SFA.DAS.LevyTransferMatching.Domain.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Domain.Extensions
{
    public static class PledgeStatusExtenstions
    {
        public static string GetLabelForPledgeStatus(this PledgeStatus status)
        {
            switch (status)
            {
                case PledgeStatus.Active: return "ACTIVE";
                case PledgeStatus.Closed: return "CLOSED";
                default:
                    return string.Empty;
            }
        }

        public static string GetCssClassForPledgeStatus(this PledgeStatus status)
        {
            switch (status)
            {
                case PledgeStatus.Active: return "govuk-tag govuk-tag--blue";
                case PledgeStatus.Closed: return "govuk-tag govuk-tag--grey";
                default:
                    return string.Empty;
            }
        }
    }
}
