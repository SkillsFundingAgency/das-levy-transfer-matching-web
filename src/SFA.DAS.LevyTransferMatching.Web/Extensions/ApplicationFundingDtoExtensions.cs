using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.ApplicationsService.Types;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class ApprenticeshipFundingDtoExtensions
    {
        public static ApprenticeshipFundingDto GetEffectiveFundingLine(
            this IEnumerable<ApprenticeshipFundingDto> apprenticeshipFunding, DateTime startDate)
        {
            return apprenticeshipFunding
                    .FirstOrDefault(c =>
                        c.EffectiveFrom <= startDate
                        && (c.EffectiveTo.IsNull() || c.EffectiveTo >= startDate)) ??
                        apprenticeshipFunding.First(c => c.EffectiveTo.IsNull());
        }

        public static int CalcFundingForDate(this ApprenticeshipFundingDto apprenticeshipFunding, int? numberOfApprentices, DateTime startDate)
        {
            if (startDate > DateTime.UtcNow.EndOfMarchOfFinancialYear())
            {
                return 0;
            }

            var net = apprenticeshipFunding.MaxEmployerLevyCap - (apprenticeshipFunding.MaxEmployerLevyCap * 0.2);
            var monthlyCost = (net / apprenticeshipFunding.Duration);
            var cost = monthlyCost * (startDate.MonthsTillFinancialYearEnd() - 1);

            return (cost * numberOfApprentices ?? 0).ToNearest(100);
        }

        public static int CalculateEstimatedTotalCost(this ApprenticeshipFundingDto apprenticeshipFunding, int numberOfApprentices)
        {
            return apprenticeshipFunding.MaxEmployerLevyCap * numberOfApprentices;
        }
    }
}
