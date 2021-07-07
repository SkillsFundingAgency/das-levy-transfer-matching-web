using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Validators
{
    public class ValidatorService : IValidatorService
    {
        private readonly ILocationService _locationService;

        public ValidatorService(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public async Task<Dictionary<int,string>> ValidateLocations(LocationPostRequest request)
        {
            var errors = new Dictionary<int, string>();

            await CheckLocationsExist(errors, request.Locations);
            CheckForDuplicates(errors, request.Locations);

            return errors;
        }

        private async Task CheckLocationsExist(Dictionary<int, string> errors, List<string> locations)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i] != null)
                {
                    var locationsDto = await _locationService.GetLocationInformation(locations[i]);
                    if (locationsDto?.Name == null)
                    {
                        if (!errors.ContainsKey(i))
                            errors.Add(i, $"No locations could be found for { locations[i] }");
                    }
                    else
                    {
                        locations[i] = locationsDto.Name;
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
