using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService
{
    public class TagService : ITagService
    {
        private readonly HttpClient _client;
        private readonly ICacheStorageService _cacheStorageService;
        private const string CacheKeyPrefix = "TagService.";
        private const int CacheExpiryInHours = 2;

        public TagService(HttpClient client, ICacheStorageService cacheStorageService)
        {
            _client = client;
            _cacheStorageService = cacheStorageService;
        }

        public async Task<List<ReferenceDataItem>> GetLevels()
        {
            return await GetFromApiWithCache("reference/levels");
        }

        public async Task<List<ReferenceDataItem>> GetSectors()
        {
            return await GetFromApiWithCache("reference/sectors");
        }

        public async Task<List<ReferenceDataItem>> GetJobRoles()
        {
            return await GetFromApiWithCache("reference/jobRoles");
        }

        private async Task<List<ReferenceDataItem>> GetFromApiWithCache(string uri)
        {
            var result = await _cacheStorageService.RetrieveFromCache<List<ReferenceDataItem>>($"{CacheKeyPrefix}{uri}");
            if (result != null) return result;

            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<List<ReferenceDataItem>>(content);

            await _cacheStorageService.SaveToCache($"{CacheKeyPrefix}{uri}", result, CacheExpiryInHours);
            return result;
        }
    }
}
