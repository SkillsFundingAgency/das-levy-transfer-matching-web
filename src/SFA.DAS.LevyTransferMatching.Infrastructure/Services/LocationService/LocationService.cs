using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient _client;

        public LocationService(HttpClient client)
        {
            _client = client;
        }

        public async Task<LocationsDto> GetLocations(string searchTerm)
        {
            var response = await _client.GetAsync($"locations?searchTerm={searchTerm}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var locationDto = JsonConvert.DeserializeObject<LocationsDto>(content);

            return locationDto;
        }

        public async Task<LocationInformationDto> GetLocationInformation(string location)
        {
            var encodedLocation = HttpUtility.UrlEncode(location);
            var response = await _client.GetAsync($"locations/information?location={encodedLocation}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var locationInformationDto = JsonConvert.DeserializeObject<LocationInformationDto>(content);

            return locationInformationDto;
        }
    }
}
