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

        public async Task<LocationDto> SearchLocation(string searchTerm)
        {
            var response = await _client.GetAsync($"/Locations?searchTerm={searchTerm}");

            return new LocationDto();
        }
    }
}
