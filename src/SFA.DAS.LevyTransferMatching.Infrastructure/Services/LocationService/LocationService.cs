using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
            var response = await _client.GetAsync($"/Locations?searchTerm={searchTerm}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var locationDto = JsonConvert.DeserializeObject<LocationsDto>(content);

            return locationDto;
        }

        public async Task<LocationInformationDto> GetLocationInformation(string location)
        {
            var response = await _client.GetAsync($"/Locations/information?location={location}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var locationInformationDto = JsonConvert.DeserializeObject<LocationInformationDto>(content);

            return locationInformationDto;
        }
    }
}
