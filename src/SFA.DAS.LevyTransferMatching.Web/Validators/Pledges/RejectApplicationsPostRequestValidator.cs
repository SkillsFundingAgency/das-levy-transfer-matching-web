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
                .WithMessage("You need to select yes if you want to continue and reject selected applications");
        }
    }
}
