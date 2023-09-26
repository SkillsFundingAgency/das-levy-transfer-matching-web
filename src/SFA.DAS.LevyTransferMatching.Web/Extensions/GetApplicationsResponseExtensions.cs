using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using System;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class GetApplicationsResponseExtensions
    {
        public static int? GetRemainingDaysForDelayedApproval(this GetApplicationsResponse.Application app, AutomaticApprovalOption automaticApprovalOption)
        {
            if (app.Status == ApplicationStatus.Pending
                && automaticApprovalOption == AutomaticApprovalOption.DelayedAutoApproval
                && app.IsJobRoleMatch && app.IsLocationMatch && app.IsSectorMatch && app.IsLevelMatch)
            {
                // doesn't apply to applications over 6 weeks old.
                DateTime sixWeeksAgo = DateTime.Today.AddDays(-42);
                if (app.CreatedOn > sixWeeksAgo)
                {
                    DateTime autoApprovalDate = app.CreatedOn.AddDays(42);
                    TimeSpan difference = autoApprovalDate - DateTime.Today;
                    int? daysUntilAutoApproval = (int)difference.TotalDays;
                    return daysUntilAutoApproval > 0 && daysUntilAutoApproval <= 7 ? daysUntilAutoApproval : null;
                }
            }
            return null;
        }

        public static int? GetRemainingDaysForAutoRejection(this GetApplicationsResponse.Application app)
        {
            if (app.Status == ApplicationStatus.Pending)
            {
                DateTime threeMonthsAgo = DateTime.Today.AddMonths(-3);

                // doesn't apply to applications over 3 months old.
                if (app.CreatedOn > threeMonthsAgo)
                {
                    TimeSpan difference = app.CreatedOn - threeMonthsAgo;
                    int? daysUntilRejection = (int)difference.TotalDays;
                    return daysUntilRejection > 0 && daysUntilRejection <= 7 ? daysUntilRejection : null;
                }
            }
            return null;
        }

        public static string GetDateDependentStatus(this GetApplicationsResponse.Application app, AutomaticApprovalOption automaticApprovalOption)
        {
            var daysTilAutoApproval = GetRemainingDaysForDelayedApproval(app, automaticApprovalOption);
            var daysTilAutoRejection = GetRemainingDaysForAutoRejection(app);
            if (daysTilAutoApproval.HasValue)
            {               
                return $"AUTO APPROVAL ON {GetAutoApprovalDate(daysTilAutoApproval.Value)}";
            }
            if (daysTilAutoRejection.HasValue)
            {                
                return $"APPLICATION EXPIRES ON {GetAutoApprovalDate(daysTilAutoRejection.Value)}";
            }

            return app.Status.ToString();
        }

        private static string GetAutoApprovalDate(int remainingDays)
        {
            DateTime futureDate = DateTime.Today.AddDays(remainingDays);
            string formattedDate = futureDate.ToString("dd MMM yyyy").ToUpper();
            return formattedDate;
        }
    }
}
