using System;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Configuration
{
    public class LevyTransferMatchingWeb
    {
        public string RedisConnectionString { get; set; }
        public string DataProtectionKeysDatabase { get; set; }
        public DateTime? UtcNowOverride { get; set; }
    }
}
