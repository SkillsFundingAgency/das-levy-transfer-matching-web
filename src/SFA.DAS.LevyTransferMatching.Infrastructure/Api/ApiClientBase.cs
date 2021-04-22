using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Domain.ApiClient;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Api
{
    public abstract class ApiClientBase
    {
        protected abstract Task<string> GetAccessTokenAsync();

        public async Task<IEnumerable<TResponse>> GetAll<TResponse>(IGetAllApiRequest request)
        {
            var accessToken = await GetAccessTokenAsync();
            using (var client = new HttpClient())//not unit testable using directly
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await client.GetAsync(request.GetAllUrl).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<IEnumerable<TResponse>>(json);
            }
        }
    }
}
