using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;

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

        public static double CalcFundingForDate(this ApprenticeshipFundingDto apprenticeshipFunding, int? numberOfApprentices, DateTime startDate)
        {
            var net = apprenticeshipFunding.MaxEmployerLevyCap - (apprenticeshipFunding.MaxEmployerLevyCap * 0.2);
            var monthlyCost = (net / apprenticeshipFunding.Duration);
            var cost = monthlyCost * startDate.MonthsTillFinancialYearEnd();

            return (cost * numberOfApprentices ?? 0).ToNearest(100);
        }
    }
}
