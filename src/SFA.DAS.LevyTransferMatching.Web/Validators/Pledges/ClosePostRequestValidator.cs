using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{
    public class ClosePostRequestValidator : AbstractValidator<ClosePostRequest>
    {
        public ClosePostRequestValidator()
        {
            RuleFor(x => x.HasConfirmed)
                .NotNull()
                .WithMessage("You need to select Yes if you want to continue and close this pledge");
        }
    }
}
