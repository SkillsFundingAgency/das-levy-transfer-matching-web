using SFA.DAS.LevyTransferMatching.Domain.Types;
using System;

namespace SFA.DAS.LevyTransferMatching.Domain.Extensions
{
    public static class ApplicationStatusExtensions
    {
        public static string GetLabelForSender(this ApplicationStatus status,  int? RemainingDaysForDelayedApproval, int? RemainingDaysForAutoRejection)
        {
            if (RemainingDaysForDelayedApproval.HasValue)
            {
                string autoApprovalDate = GetAutoApprovalDate(RemainingDaysForDelayedApproval.Value);
                return $"AUTO APPROVAL ON {autoApprovalDate}";
            }

            if (RemainingDaysForAutoRejection.HasValue)
            {
                string autoApprovalDate = GetAutoApprovalDate(RemainingDaysForAutoRejection.Value);
                return $"APPLICATION EXPIRES ON {autoApprovalDate}";
            }

            switch (status)
            {
                case ApplicationStatus.Pending: return "AWAITING YOUR APPROVAL";
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

        private static string GetAutoApprovalDate(int remainingDays)
        {
            DateTime futureDate = DateTime.Today.AddDays(remainingDays);
            string formattedDate = futureDate.ToString("dd MMM yyyy").ToUpper();
            return formattedDate;
        }

        public static string GetCssClassForSender(this ApplicationStatus status, int? RemainingDaysForDelayedApproval, int? RemainingDaysForAutoRejection)
        {
            if (RemainingDaysForDelayedApproval.HasValue)
            {
                return "govuk-tag govuk-tag--yellow";
            }

            if (RemainingDaysForAutoRejection.HasValue)
            {
                return "govuk-tag govuk-tag--orange";
            }
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

        public static string GetLabelForReceiverForApplications(this ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Pending => "AWAITING APPROVAL",
                ApplicationStatus.Approved => "APPROVED, AWAITING YOUR ACCEPTANCE",
                ApplicationStatus.Rejected => "REJECTED",
                ApplicationStatus.Accepted => "FUNDS AVAILABLE",
                ApplicationStatus.FundsUsed => "FUNDS USED",
                ApplicationStatus.Declined => "WITHDRAWN",
                ApplicationStatus.Withdrawn => "WITHDRAWN",
                ApplicationStatus.WithdrawnAfterAcceptance => "WITHDRAWN",
                _ => string.Empty,
            };
        }

        public static string GetLabelForReceiver(this ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Pending => "Awaiting approval",
                ApplicationStatus.Approved => "Approved, awaiting your acceptance",
                ApplicationStatus.Rejected => "Rejected",
                ApplicationStatus.Accepted => "Funds available to add apprentice",
                ApplicationStatus.FundsUsed => "Funds used",
                ApplicationStatus.Declined => "Withdrawn",
                ApplicationStatus.Withdrawn => "Withdrawn",
                ApplicationStatus.WithdrawnAfterAcceptance => "Withdrawn",
                _ => string.Empty,
            };
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
