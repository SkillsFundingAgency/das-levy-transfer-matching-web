using System;
using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Domain.Extensions;

public static class ApplicationStatusExtensions
{
    public static string GetLabelForSender(this ApplicationStatus status, int? remainingDaysForDelayedApproval, int? remainingDaysForAutoRejection)
    {
        if (remainingDaysForDelayedApproval.HasValue)
        {
            var autoApprovalDate = GetAutoApprovalDate(remainingDaysForDelayedApproval.Value);
            return $"AUTO APPROVAL ON {autoApprovalDate}";
        }

        if (remainingDaysForAutoRejection.HasValue)
        {
            var autoApprovalDate = GetAutoApprovalDate(remainingDaysForAutoRejection.Value);
            return $"APPLICATION EXPIRES ON {autoApprovalDate}";
        }

        return status switch
        {
            ApplicationStatus.Pending => "AWAITING YOUR APPROVAL",
            ApplicationStatus.Approved => "AWAITING ACCEPTANCE BY APPLICANT",
            ApplicationStatus.Accepted => "OFFER OF FUNDING ACCEPTED",
            ApplicationStatus.FundsUsed => "FUNDS USED",
            ApplicationStatus.Rejected => "REJECTED",
            ApplicationStatus.Declined => "WITHDRAWN BY APPLICANT",
            ApplicationStatus.Withdrawn => "WITHDRAWN BY APPLICANT",
            ApplicationStatus.WithdrawnAfterAcceptance => "WITHDRAWN BY APPLICANT",
            _ => string.Empty
        };
    }

    private static string GetAutoApprovalDate(int remainingDays)
    {
        var futureDate = DateTime.Today.AddDays(remainingDays);
        var formattedDate = futureDate.ToString("dd MMM yyyy").ToUpper();
        return formattedDate;
    }

    public static string GetCssClassForSender(this ApplicationStatus status, int? remainingDaysForDelayedApproval, int? remainingDaysForAutoRejection)
    {
        if (remainingDaysForDelayedApproval.HasValue)
        {
            return "govuk-tag govuk-tag--yellow";
        }

        if (remainingDaysForAutoRejection.HasValue)
        {
            return "govuk-tag govuk-tag--orange";
        }

        return status switch
        {
            ApplicationStatus.Pending => "govuk-tag govuk-tag--blue",
            ApplicationStatus.Approved => "govuk-tag govuk-tag--yellow",
            ApplicationStatus.Accepted => "govuk-tag govuk-tag--turquoise",
            ApplicationStatus.FundsUsed => "govuk-tag govuk-tag--pink",
            ApplicationStatus.Rejected => "govuk-tag govuk-tag--red",
            ApplicationStatus.Declined => "govuk-tag govuk-tag--yellow",
            ApplicationStatus.Withdrawn => "govuk-tag govuk-tag--yellow",
            ApplicationStatus.WithdrawnAfterAcceptance => "govuk-tag govuk-tag--yellow",
            _ => string.Empty
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
        return status switch
        {
            ApplicationStatus.Pending => "govuk-tag govuk-tag--grey",
            ApplicationStatus.Approved => "govuk-tag govuk-tag--blue",
            ApplicationStatus.Rejected => "govuk-tag govuk-tag--red",
            ApplicationStatus.Accepted => "govuk-tag govuk-tag--turquoise",
            ApplicationStatus.FundsUsed => "govuk-tag govuk-tag--pink",
            ApplicationStatus.Declined => "govuk-tag govuk-tag--yellow",
            ApplicationStatus.Withdrawn => "govuk-tag govuk-tag--yellow",
            ApplicationStatus.WithdrawnAfterAcceptance => "govuk-tag govuk-tag--yellow",
            _ => string.Empty
        };
    }
}