using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Exceptions;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService
{
    public static class ResponseHelpers
    {
        public static async Task<T> HandleDeserialisationOrThrow<T>(this HttpResponseMessage response) where T : class
        {
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NullModelException(nameof(T));
            }

            response.EnsureSuccessStatusCode();
            return null;
        }
    }
}