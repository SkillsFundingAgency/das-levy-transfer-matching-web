using System;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Configuration
{
    public class LevyTransferMatchingWeb
    {
        public string RedisConnectionString { get; set; }
        public string DataProtectionKeysDatabase { get; set; }
        public string UtcNowOverride { get; set; }
        public bool IsLive { get; set; }
        public bool UseGovSignIn { get; set; }
    }
}