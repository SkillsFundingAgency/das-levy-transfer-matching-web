using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Applications
{
    public class ApplicationPostRequestValidator : AbstractValidator<ApplicationPostRequest>
    {
        public ApplicationPostRequestValidator()
        {
            RuleFor(o => o.SelectedAction)
                .Equal(ApplicationViewModel.ApprovalAction.Accept)
                .Equal(ApplicationViewModel.ApprovalAction.None)
                .WithMessage("You must select to either withdraw the application or keep the application")
                .When(o => o.SelectedAction == null && o.CanWithdraw);

            RuleFor(o => o.HasAcceptedTermsAndConditions)
                .Equal(true)
                .WithMessage("You must agree to the terms and conditions before accepting funding for this application")
                .When(o => o.SelectedAction.HasValue && o.CanAcceptFunding);

            RuleFor(o => o.SelectedAction)
                .Equal(ApplicationViewModel.ApprovalAction.Accept)
                .WithMessage("You must choose to either accept or decline funding for this application")
                .When(o => o.SelectedAction == null && o.CanAcceptFunding);

        }
    }
}