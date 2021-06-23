using System;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}