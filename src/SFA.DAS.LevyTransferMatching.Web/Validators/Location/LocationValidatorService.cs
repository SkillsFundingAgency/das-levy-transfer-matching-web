using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Location;

public interface ILocationValidatorService
{
    Task<Dictionary<int, string>> ValidateLocations(LocationPostRequest request, IDictionary<int, IEnumerable<string>> multipleValidResults);
}
    
public class LocationValidatorService : ILocationValidatorService
{
    private readonly ILocationService _locationService;

    public LocationValidatorService(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task<Dictionary<int, string>> ValidateLocations(LocationPostRequest request, IDictionary<int, IEnumerable<string>> multipleValidResults)
    {
        var errors = new Dictionary<int, string>();

        await CheckLocationsExist(request.Locations, errors, multipleValidResults);
        CheckForDuplicateLocations(errors, request.Locations);

        return errors;
    }

    private async Task CheckLocationsExist(List<string> locations, Dictionary<int, string> errors, IDictionary<int, IEnumerable<string>> multipleValidResults)
    {
        for (var index = 0; index < locations.Count; index++)
        {
            if (locations[index] != null)
            {
                await CheckLocationExists(locations, errors, multipleValidResults, index);
            }
        }
    }

    private async Task CheckLocationExists(List<string> locations, Dictionary<int, string> errors, IDictionary<int, IEnumerable<string>> multipleValidResults, int i)
    {
        var locationSuggestions = await _locationService.GetLocations(locations[i]);

        if (!locationSuggestions.Names.Any())
        {
            await CheckIfValidLocationSuggestion(errors, locations, i);
        }
        else if (locationSuggestions.Names.Count() == 1)
        {
            var locationInformation = await _locationService.GetLocationInformation(locations[i]);

            locations[i] = locationInformation.Name;
        }
        else
        {
            multipleValidResults.Add(i, locationSuggestions.Names);
        }
    }

    private async Task CheckIfValidLocationSuggestion(Dictionary<int, string> errors, List<string> locations, int index)
    {
        var locationInformation = await _locationService.GetLocationInformation(locations[index]);

        if (string.IsNullOrEmpty(locationInformation?.Name))
        {
            errors.Add(index, $"Check the spelling of your location");
        }
    }

    private static void CheckForDuplicateLocations(Dictionary<int, string> errors, List<string> locations)
    {
        for (var index = 0; index < locations.Count; index++)
        {
            if (locations[index] == null)
            {
                continue;
            }

            if (locations.Count(x => x == locations[index]) <= 1)
            {
                continue;
            }
            
            errors.TryAdd(index, "Duplicates of the same location are not allowed");
        }
    }
}