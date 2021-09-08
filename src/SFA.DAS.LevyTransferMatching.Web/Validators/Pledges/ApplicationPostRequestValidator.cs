using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{
    public class ApplicationPostRequestValidator : AbstractValidator<ApplicationPostRequest>
    {
        public ApplicationPostRequestValidator()
        {
            RuleFor(x => x.SelectedAction).NotEmpty().WithMessage("You must choose to either approve or reject this application");
        }
    }
}
