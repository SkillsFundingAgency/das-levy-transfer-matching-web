using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{
    public class ApplicationApprovalOptionsPostRequestValidator : AbstractValidator<ApplicationApprovalOptionsPostRequest>
    {
        public ApplicationApprovalOptionsPostRequestValidator()
        {
            RuleFor(x => x.AutomaticApproval)
                .NotNull()
                .WithMessage("You must choose to either 'Check and review apprentice details' or 'Automatically approve the apprentice details'");
        }
    }
}
