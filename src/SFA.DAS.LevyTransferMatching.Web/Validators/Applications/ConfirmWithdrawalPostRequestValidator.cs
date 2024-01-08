using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Applications;

public class ConfirmWithdrawalPostRequestValidator : AbstractValidator<ConfirmWithdrawalPostRequest>
{
    public ConfirmWithdrawalPostRequestValidator()
    {
        RuleFor(x => x.HasConfirmed)
            .NotNull()
            .WithMessage("You must confirm that you want to withdraw the application");
    }
}