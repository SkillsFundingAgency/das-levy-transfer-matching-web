using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;

public class LocationService(HttpClient client) : ILocationService
{
    public async Task<LocationsDto> GetLocations(string searchTerm)
    {
        var response = await client.GetAsync($"locations?searchTerm={searchTerm}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var locationDto = JsonConvert.DeserializeObject<LocationsDto>(content);

        return locationDto;
    }

    public async Task<LocationInformationDto> GetLocationInformation(string location)
    {
        var encodedLocation = WebUtility.UrlEncode(location);
        var response = await client.GetAsync($"locations/information?location={encodedLocation}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var locationInformationDto = JsonConvert.DeserializeObject<LocationInformationDto>(content);

        return locationInformationDto;
    }
}