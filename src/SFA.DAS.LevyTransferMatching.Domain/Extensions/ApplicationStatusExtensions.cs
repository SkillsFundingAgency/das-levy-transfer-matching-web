using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Domain.Extensions
{
    public static class ApplicationStatusExtensions
    {
        public static string GetLabelForSender(this ApplicationStatus status)
        {
            switch (status)
            {
                case ApplicationStatus.Pending: return "AWAITING APPROVAL";
                case ApplicationStatus.Approved: return "AWAITING ACCEPTANCE BY APPLICANT";
                case ApplicationStatus.Accepted: return "OFFER OF FUNDING ACCEPTED";
                case ApplicationStatus.FundsUsed: return "FUNDS USED";
                case ApplicationStatus.Rejected: return "REJECTED";
                case ApplicationStatus.Declined: return "WITHDRAWN BY APPLICANT";
                case ApplicationStatus.Withdrawn: return "WITHDRAWN BY APPLICANT";
                case ApplicationStatus.WithdrawnAfterAcceptance: return "WITHDRAWN BY APPLICANT";
                default:
                    return string.Empty;
            }
        }

        public static string GetCssClassForSender(this ApplicationStatus status)
        {
            switch (status)
            {
                case ApplicationStatus.Pending: return "govuk-tag govuk-tag--blue";
                case ApplicationStatus.Approved: return "govuk-tag govuk-tag--yellow";
                case ApplicationStatus.Accepted: return "govuk-tag govuk-tag--turquoise";
                case ApplicationStatus.FundsUsed: return "govuk-tag govuk-tag--pink";
                case ApplicationStatus.Rejected: return "govuk-tag govuk-tag--red";
                case ApplicationStatus.Declined: return "govuk-tag govuk-tag--yellow";
                case ApplicationStatus.Withdrawn: return "govuk-tag govuk-tag--yellow";
                case ApplicationStatus.WithdrawnAfterAcceptance: return "govuk-tag govuk-tag--yellow";
                default:
                    return string.Empty;
            }
        }

        public static string GetLabelForReceiver(this ApplicationStatus status)
        {
            switch (status)
            {
                case ApplicationStatus.Pending: return "AWAITING APPROVAL";
                case ApplicationStatus.Approved: return "APPROVED, AWAITING YOUR ACCEPTANCE";
                case ApplicationStatus.Rejected: return "REJECTED";
                case ApplicationStatus.Accepted: return "FUNDS AVAILABLE";
                case ApplicationStatus.FundsUsed: return "FUNDS USED";
                case ApplicationStatus.Declined: return "WITHDRAWN";
                case ApplicationStatus.Withdrawn: return "WITHDRAWN";
                case ApplicationStatus.WithdrawnAfterAcceptance: return "WITHDRAWN";
                default:
                    return string.Empty;
            }
        }

        public static string GetCssClassForReceiver(this ApplicationStatus status)
        {
            switch (status)
            {
                case ApplicationStatus.Pending: return "govuk-tag govuk-tag--grey";
                case ApplicationStatus.Approved: return "govuk-tag govuk-tag--blue";
                case ApplicationStatus.Rejected: return "govuk-tag govuk-tag--red";
                case ApplicationStatus.Accepted: return "govuk-tag govuk-tag--blue";
                case ApplicationStatus.FundsUsed: return "govuk-tag govuk-tag--pink";
                case ApplicationStatus.Declined: return "govuk-tag govuk-tag--yellow";
                case ApplicationStatus.Withdrawn: return "govuk-tag govuk-tag--yellow";
                case ApplicationStatus.WithdrawnAfterAcceptance: return "govuk-tag govuk-tag--yellow";
                default:
                    return string.Empty;
            }
        }
    }
}
