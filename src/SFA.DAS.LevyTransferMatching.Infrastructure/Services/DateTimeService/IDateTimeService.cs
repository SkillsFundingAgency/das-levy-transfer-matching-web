using System;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService
{
    public interface IDateTimeService
    {
        DateTime UtcNow { get; }
    }
}