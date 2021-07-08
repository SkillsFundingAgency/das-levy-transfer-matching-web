using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{
    public class LocationPostRequestValidator : AbstractValidator<LocationPostRequest>
    {
        public LocationPostRequestValidator()
        {
            RuleForEach(x => x.Locations)
                .Must((model, location) => IsLocationUnique(model.Locations, location))
                .WithMessage("Duplicates of the same location are not allowed");
        }

        private bool IsLocationUnique(List<string> locationsList, string locationName)
        {
            if (locationName == null)
                return true;

            return locationsList.Count(x => x == locationName) == 1;
        }
    }
}
