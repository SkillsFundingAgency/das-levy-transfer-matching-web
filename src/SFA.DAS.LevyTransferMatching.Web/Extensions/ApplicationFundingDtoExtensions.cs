﻿using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions;

public static class ApprenticeshipFundingDtoExtensions
{
    public static ApprenticeshipFundingDto GetEffectiveFundingLine(this IEnumerable<ApprenticeshipFundingDto> apprenticeshipFunding, DateTime startDate)
    {
        // Preventing possible multiple enumerations
        var apprenticeshipFundingArray = apprenticeshipFunding as ApprenticeshipFundingDto[] ?? apprenticeshipFunding.ToArray();
        
        return apprenticeshipFundingArray
                   .FirstOrDefault(c =>
                       c.EffectiveFrom <= startDate
                       && (c.EffectiveTo.IsNull() || c.EffectiveTo >= startDate))
               ?? apprenticeshipFundingArray.First(c => c.EffectiveTo.IsNull());
    }

    public static int CalculateEstimatedTotalCost(this ApprenticeshipFundingDto apprenticeshipFunding, int numberOfApprentices)
    {
        return apprenticeshipFunding.MaxEmployerLevyCap * numberOfApprentices;
    }

    public static int CalculateOneYearCost(this ApprenticeshipFundingDto apprenticeshipFunding, int numberOfApprentices)
    {
        if (numberOfApprentices == 0) return 0;

        if (apprenticeshipFunding.Duration <= 12)
        {
            return apprenticeshipFunding.MaxEmployerLevyCap * numberOfApprentices;
        }

        var fundingBandMax = ((double)apprenticeshipFunding.MaxEmployerLevyCap * numberOfApprentices) * 0.8;
        
        return (fundingBandMax / apprenticeshipFunding.Duration * 12).ToNearest(1);
    }
}