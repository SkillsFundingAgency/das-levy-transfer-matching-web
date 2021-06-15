using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService
{
    public class TagService : ITagService
    {
        private readonly HttpClient _client;

        public TagService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Tag>> GetLevels()
        {
            var response = await _client.GetAsync($"tags/levels");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Tag>>(content);
        }

        public async Task<List<Tag>> GetSectors()
        {
            var response = await _client.GetAsync($"tags/sectors");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Tag>>(content);
        }

        public async Task<List<Tag>> GetJobRoles()
        {
            var response = await _client.GetAsync($"tags/jobRoles");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Tag>>(content);
        }
    }
}
