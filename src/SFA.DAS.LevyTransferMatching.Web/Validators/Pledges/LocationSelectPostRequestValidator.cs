using FluentValidation;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{
    public class LocationSelectPostRequestValidator : AbstractValidator<LocationSelectPostRequest>
    {
        public LocationSelectPostRequestValidator()
        {
            RuleForEach(x => x.LocationSelectionGroups)
                .Must(x => !string.IsNullOrEmpty(x.SelectedValue))
                .WithMessage("Please select a location");
        }
    }
}