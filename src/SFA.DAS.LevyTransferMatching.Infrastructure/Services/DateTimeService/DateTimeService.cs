using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using System;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService
{
    public class DateTimeService : IDateTimeService
    {
        private readonly LevyTransferMatchingWeb _levyTransferMatchingWeb;

        public DateTimeService(LevyTransferMatchingWeb levyTransferMatchingWeb)
        {
            _levyTransferMatchingWeb = levyTransferMatchingWeb;
        }

        public DateTime UtcNow
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_levyTransferMatchingWeb.UtcNowOverride))
                {
                    return DateTime.Parse(_levyTransferMatchingWeb.UtcNowOverride);
                }
                else
                {
                    return DateTime.UtcNow;
                }
            }
        }
    }
}