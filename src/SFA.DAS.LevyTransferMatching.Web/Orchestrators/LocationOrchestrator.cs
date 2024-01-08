using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Web.Models.Location;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators;

public class LocationOrchestrator : ILocationOrchestrator
{
    private readonly ILocationService _locationService;

    public LocationOrchestrator(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task<LocationsTypeAheadViewModel> GetLocationsTypeAheadViewModel(string searchTerm)
    {
        var locationsDto = await _locationService.GetLocations(searchTerm);

        return new LocationsTypeAheadViewModel
        {
            Locations = locationsDto.Names.Select(x => new LocationTypeAheadViewModel { Name = x }).ToList()
        };
    }
}