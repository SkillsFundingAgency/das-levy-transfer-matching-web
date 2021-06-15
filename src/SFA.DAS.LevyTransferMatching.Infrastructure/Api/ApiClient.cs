using System;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Domain.Interfaces;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Api
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly LevyTransferMatchingApi _config;

        public ApiClient(HttpClient httpClient, LevyTransferMatchingApi config)
        {
            _httpClient = httpClient;
            _config = config;
            _httpClient.BaseAddress = new Uri(_config.ApiBaseUrl);
        }

        public async Task Ping()
        {
            AddHeaders();
            await _httpClient.GetAsync("/ping");
        }

        public Task<TResponse> Get<TResponse>(object request)
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> Post<TResponse, TPostData>(object request)
        {
            throw new NotImplementedException();
        }

        private void AddHeaders()
        {
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _config.SubscriptionKey);
            _httpClient.DefaultRequestHeaders.Add("X-Version", "1");
        }
    }
}
