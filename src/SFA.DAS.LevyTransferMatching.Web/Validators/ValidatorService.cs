using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System.Collections.Generic;
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
            for (int i = 0; i < request.Locations.Count; i++)
            {
                if (request.Locations[i] != null)
                {
                    var locationsDto = await _locationService.GetLocationInformation(request.Locations[i]);
                    if (locationsDto?.Name == null)
                    {
                        errors.Add(i, $"No locations could be found for { request.Locations[i] }");
                    }
                    else
                    {
                        request.Locations[i] = locationsDto.Name;
                    }
                }
            }

            return errors;
        }
    }
}
