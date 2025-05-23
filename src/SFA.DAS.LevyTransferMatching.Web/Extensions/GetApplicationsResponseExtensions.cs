﻿using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions;

public static class GetApplicationsResponseExtensions
{
    public static int? GetRemainingDaysForDelayedApproval(this GetApplicationsResponse.Application app, AutomaticApprovalOption automaticApprovalOption)
    {
        if (app.Status != ApplicationStatus.Pending
            || automaticApprovalOption != AutomaticApprovalOption.DelayedAutoApproval
            || !app.IsJobRoleMatch || !app.IsLocationMatch || !app.IsSectorMatch || !app.IsLevelMatch)
        {
            return null;
        }
        
        // doesn't apply to applications over 6 weeks old.
        var sixWeeksAgo = DateTime.Today.AddDays(-42);
        
        if (app.CreatedOn <= sixWeeksAgo)
        {
            return null;
        }
        
        var autoApprovalDate = app.CreatedOn.AddDays(42);
        var difference = autoApprovalDate - DateTime.Today;
        int? daysUntilAutoApproval = (int)difference.TotalDays;
        
        return daysUntilAutoApproval is > 0 and <= 7 ? daysUntilAutoApproval : null;
    }

    public static int? GetRemainingDaysForAutoRejection(this GetApplicationsResponse.Application app)
    {
        if (app.Status != ApplicationStatus.Pending)
        {
            return null;
        }
        
        var threeMonthsAgo = DateTime.Today.AddMonths(-3);

        // doesn't apply to applications over 3 months old.
        if (app.CreatedOn <= threeMonthsAgo)
        {
            return null;
        }
        
        var difference = app.CreatedOn - threeMonthsAgo;
        int? daysUntilRejection = (int)difference.TotalDays;
        
        return daysUntilRejection is > 0 and <= 7 ? daysUntilRejection : null;
    }

    public static string GetDateDependentStatus(this GetApplicationsResponse.Application app, AutomaticApprovalOption automaticApprovalOption)
    {
        var daysTilAutoApproval = GetRemainingDaysForDelayedApproval(app, automaticApprovalOption);
        var daysTilAutoRejection = GetRemainingDaysForAutoRejection(app);
        if (daysTilAutoApproval.HasValue)
        {               
            return $"Auto approval on {GetAutoApprovalDate(daysTilAutoApproval.Value)}";
        }
        if (daysTilAutoRejection.HasValue)
        {                
            return $"Application expires on {GetAutoApprovalDate(daysTilAutoRejection.Value)}";
        }

        if (app.Status == ApplicationStatus.Approved)
        {
            if (automaticApprovalOption == AutomaticApprovalOption.NotApplicable)
            {
                return "Awaiting acceptance by applicant";
            }

            string prefix = automaticApprovalOption == AutomaticApprovalOption.DelayedAutoApproval ? "Delayed" : "Auto";
            return $"{prefix} approval: Awaiting acceptance by applicant";
        }

        return app.Status switch
        {
            ApplicationStatus.Pending => "Awaiting your approval",
            ApplicationStatus.Accepted => "Offer of funding accepted",
            ApplicationStatus.FundsUsed => "Funds used",
            ApplicationStatus.Rejected => "Rejected",
            ApplicationStatus.Declined => "Declined by applicant",
            ApplicationStatus.Withdrawn => "Withdrawn by applicant",
            ApplicationStatus.WithdrawnAfterAcceptance => "Withdrawn by applicant",
            ApplicationStatus.FundsExpired => "Funds no longer available",
            _ => app.Status.ToString()
        };
    }

    private static string GetAutoApprovalDate(int remainingDays)
    {
        var futureDate = DateTime.Today.AddDays(remainingDays);

        return futureDate.ToString("dd MMM yyyy").ToUpper();
    }
}