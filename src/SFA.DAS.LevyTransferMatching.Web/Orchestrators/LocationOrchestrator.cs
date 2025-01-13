using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Web.Models.Location;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators;

public class LocationOrchestrator(ILocationService locationService) : ILocationOrchestrator
{
    public async Task<LocationsTypeAheadViewModel> GetLocationsTypeAheadViewModel(string searchTerm)
    {
        var locationsDto = await locationService.GetLocations(searchTerm);

        return new LocationsTypeAheadViewModel
        {
            Locations = locationsDto.Names.Select(x => new LocationTypeAheadViewModel { Name = x }).ToList()
        };
    }
}