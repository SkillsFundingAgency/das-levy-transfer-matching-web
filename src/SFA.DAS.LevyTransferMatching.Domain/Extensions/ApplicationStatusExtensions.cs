using System;
using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Domain.Extensions
{
    public static class ApplicationStatusExtensions
    {
        public static string GetLabelForSender(this ApplicationStatus status,
            AutomaticApprovalOption automaticApprovalOption, 
            int? RemainingDaysForDelayedApproval, 
            int? RemainingDaysForAutoRejection)
        {
            if (RemainingDaysForDelayedApproval.HasValue)
            {
                string autoApprovalDate = GetAutoApprovalDate(RemainingDaysForDelayedApproval.Value);
                return $"Auto approval on {autoApprovalDate}";
            }

            if (RemainingDaysForAutoRejection.HasValue)
            {
                string autoApprovalDate = GetAutoApprovalDate(RemainingDaysForAutoRejection.Value);
                return $"Application expires on {autoApprovalDate}";
            }

            if (status == ApplicationStatus.Approved)
            {
                if (automaticApprovalOption == AutomaticApprovalOption.NotApplicable)
                {
                    return "Awaiting acceptance by applicant";
                }

                string prefix = automaticApprovalOption == AutomaticApprovalOption.DelayedAutoApproval ? "Delayed" : "Auto";
                return $"{prefix} approval: Awaiting acceptance by applicant";
            }

            return status switch
            {
                ApplicationStatus.Pending => "Awaiting your approval",
                ApplicationStatus.Accepted => "Offer of funding accepted",
                ApplicationStatus.FundsUsed => "Funds used",
                ApplicationStatus.Rejected => "Rejected",
                ApplicationStatus.Declined => "Declined by applicant",
                ApplicationStatus.Withdrawn => "Withdrawn by applicant",
                ApplicationStatus.WithdrawnAfterAcceptance => "Withdrawn by applicant",
                ApplicationStatus.FundsExpired => "Funds no longer available",
                _ => string.Empty,
            };
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

            return status switch
            {
                ApplicationStatus.Pending => "govuk-tag govuk-tag--blue",
                ApplicationStatus.Approved => "govuk-tag govuk-tag--yellow",
                ApplicationStatus.Accepted => "govuk-tag govuk-tag--green",
                ApplicationStatus.FundsUsed => "govuk-tag govuk-tag--pink",
                ApplicationStatus.Rejected => "govuk-tag govuk-tag--grey",
                ApplicationStatus.Declined => "govuk-tag govuk-tag--grey",
                ApplicationStatus.Withdrawn => "govuk-tag govuk-tag--grey",
                ApplicationStatus.WithdrawnAfterAcceptance => "govuk-tag govuk-tag--grey",
                ApplicationStatus.FundsExpired => "govuk-tag govuk-tag--grey",
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
                case ApplicationStatus.Accepted: return "govuk-tag govuk-tag--turquoise";
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
