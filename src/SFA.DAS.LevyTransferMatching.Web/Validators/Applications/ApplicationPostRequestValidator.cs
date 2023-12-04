using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Applications;

public class ApplicationPostRequestValidator : AbstractValidator<ApplicationPostRequest>
{
    public ApplicationPostRequestValidator()
    {
        RuleFor(o => o.IsWithdrawalConfirmed)
            .Equal(true)
            .WithMessage("You must confirm that you want to withdraw the application")
            .When(o => o.SelectedAction == ApplicationViewModel.ApprovalAction.Withdraw);

        RuleFor(o => o.SelectedAction)
            .NotNull()
            .WithMessage("You must select to either withdraw the application or keep the application")
            .When(o => o.CanWithdraw);

        RuleFor(o => o.SelectedAction)
            .NotNull()
            .WithMessage("You must choose to either accept or decline funding for this application")
            .When(o => o.CanAcceptFunding);

        RuleFor(o => o.HasAcceptedTermsAndConditions)
            .Equal(true)
            .WithMessage("You must agree to the terms and conditions before accepting funding for this application")
            .When(o => o.SelectedAction.HasValue && o.SelectedAction.Value == ApplicationViewModel.ApprovalAction.Accept);

        RuleFor(x => x.IsDeclineConfirmed)
            .Equal(true)
            .WithMessage("You must confirm that you want to decline the funding and withdraw the application")
            .When(x => x.SelectedAction.HasValue && x.SelectedAction.Value == ApplicationViewModel.ApprovalAction.Decline);

    }
}