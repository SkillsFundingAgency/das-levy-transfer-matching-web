using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{
    public class RejectApplicationsPostRequestValidator : AbstractValidator<RejectApplicationsPostRequest>
    {
        public RejectApplicationsPostRequestValidator()
        {
            RuleFor(x => x.RejectConfirm)
                .NotNull()
                .WithMessage("You must choose to either reject the selected applications or return to the list of applications");
        }
    }
}
