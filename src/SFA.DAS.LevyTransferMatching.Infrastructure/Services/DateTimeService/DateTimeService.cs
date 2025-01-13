using System;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.DateTimeService;

public class DateTimeService(LevyTransferMatchingWeb levyTransferMatchingWeb) : IDateTimeService
{
    public DateTime UtcNow => !string.IsNullOrWhiteSpace(levyTransferMatchingWeb.UtcNowOverride)
        ? DateTime.Parse(levyTransferMatchingWeb.UtcNowOverride)
        : DateTime.UtcNow;
}