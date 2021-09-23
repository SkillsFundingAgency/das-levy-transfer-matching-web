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
                default:
                    return string.Empty;
            }
        }
    }
}
