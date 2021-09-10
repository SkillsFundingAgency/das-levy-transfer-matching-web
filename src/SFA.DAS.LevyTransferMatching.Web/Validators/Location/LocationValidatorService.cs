using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Location
{
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

            await CheckLocationsExist(errors, request.Locations, multipleValidResults);
            CheckForDuplicates(errors, request.Locations);

            return errors;
        }

        private async Task CheckLocationsExist(Dictionary<int, string> errors, List<string> locations, IDictionary<int, IEnumerable<string>> multipleValidResults)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i] != null)
                {
                    var possibleLocations = await _locationService.GetLocations(locations[i]);

                    if (!possibleLocations.Names.Any())
                    {
                        // Either there's a spelling mistake, or this is a
                        // valid location provided by the typeahead -
                        // GetLocations will not return valid entries.
                        var locationInformation = await _locationService.GetLocationInformation(locations[i]);

                        if (string.IsNullOrEmpty(locationInformation?.Name))
                        {
                            // This isn't valid.
                            if (!errors.ContainsKey(i))
                                errors.Add(i, $"Check the spelling of your location");
                        }
                    }
                    else if (possibleLocations.Names.Count() == 1)
                    {
                        // There's exactly one result - update the Locations
                        // list, and return no error
                        var locationInformation = await _locationService.GetLocationInformation(locations[i]);

                        locations[i] = locationInformation.Name;
                    }
                    else
                    {
                        // There's multiple (valid) results to select from for
                        // this location search term -
                        // Bubble these up
                        multipleValidResults.Add(i, possibleLocations.Names);
                    }
                }
            }
        }

        private void CheckForDuplicates(Dictionary<int, string> errors, List<string> locations)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i] != null)
                {
                    if (locations.Count(x => x == locations[i]) > 1)
                    {
                        if (!errors.ContainsKey(i))
                            errors.Add(i, $"Duplicates of the same location are not allowed");
                    }
                }
            }
        }
    }
}