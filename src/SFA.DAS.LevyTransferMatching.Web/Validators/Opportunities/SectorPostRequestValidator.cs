using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    public class SectorPostRequestValidator : AbstractValidator<SectorPostRequest>
    {
        public SectorPostRequestValidator()
        {
            RuleFor(x => x.Sectors)
                .NotNull().WithMessage("Select one or more business sectors to describe your business")
                .NotEmpty().WithMessage("Select one or more business sectors to describe your business");

            RuleFor(x => x.Locations)
                .NotEmpty().WithMessage("You must select or enter a location")
                .When(x => x.HasPledgeLocations && !x.AdditionalLocation);

            RuleFor(x => x.AdditionalLocationText)
                .Must((request, s) => !string.IsNullOrWhiteSpace(request.AdditionalLocationText))
                .WithMessage("You must enter a location")
                .When(x => x.AdditionalLocation && x.HasPledgeLocations);

            RuleFor(x => x.SpecificLocation)
                .NotEmpty().WithMessage("You must select or enter a location")
                .When(x => !x.HasPledgeLocations);
        }
    }
}