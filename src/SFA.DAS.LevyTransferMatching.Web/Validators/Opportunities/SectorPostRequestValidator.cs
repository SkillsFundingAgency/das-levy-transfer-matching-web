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
        }
    }
}