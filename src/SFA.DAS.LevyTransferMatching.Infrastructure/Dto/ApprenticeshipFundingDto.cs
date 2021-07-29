using System;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Dto
{
    public class ApprenticeshipFundingDto
    {
        public int MaxEmployerLevyCap { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public int Duration { get; set; }
    }
}
