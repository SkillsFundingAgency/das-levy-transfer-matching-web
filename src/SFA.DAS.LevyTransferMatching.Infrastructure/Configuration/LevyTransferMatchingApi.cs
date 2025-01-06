using SFA.DAS.Http.Configuration;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

public class LevyTransferMatchingApi : IApimClientConfiguration
{
    public string ApiBaseUrl { get; set; }
    public string SubscriptionKey { get; set; }

    public string ApiVersion { get; set; }
}