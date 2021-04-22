using SFA.DAS.LevyTransferMatching.Domain.ApiClient;

namespace SFA.DAS.LevyTransferMatching.Domain.EmployerAccounts
{
    public class PingRequest : IGetApiRequest
    {
        public PingRequest(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; }
        public string GetUrl => $"{BaseUrl}/ping";
    }
}
