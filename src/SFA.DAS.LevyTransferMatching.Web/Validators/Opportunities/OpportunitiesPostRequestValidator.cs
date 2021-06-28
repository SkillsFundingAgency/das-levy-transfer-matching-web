using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    public class OpportunitiesPostRequestValidator : AbstractValidator<OpportunitiesPostRequest>
    {
        public OpportunitiesPostRequestValidator()
        {
            RuleFor(x => x.HasConfirmed)
                .NotNull()
                .WithMessage("You need to select Yes if you want to continue and apply for transfer funds");
        }
    }
}
