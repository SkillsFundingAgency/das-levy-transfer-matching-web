using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;

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

        public async Task<List<Tag>> GetLevels()
        {
            return await GetFromApiWithCache("tags/levels");
        }

        public async Task<List<Tag>> GetSectors()
        {
            return await GetFromApiWithCache("tags/sectors");
        }

        public async Task<List<Tag>> GetJobRoles()
        {
            return await GetFromApiWithCache("tags/jobRoles");
        }

        private async Task<List<Tag>> GetFromApiWithCache(string uri)
        {
            var result = await _cacheStorageService.RetrieveFromCache<List<Tag>>($"{CacheKeyPrefix}{uri}");
            if (result != null) return result;

            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<List<Tag>>(content);

            await _cacheStorageService.SaveToCache($"{CacheKeyPrefix}{uri}", result, CacheExpiryInHours);
            return result;
        }
    }
}
