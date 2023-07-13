using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System.Globalization;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{
    public class AutoApprovePostRequestValidator : AbstractValidator<AutoApprovePostRequest>
    {
        public AutoApprovePostRequestValidator()
        {           
            RuleFor(x => x.AutoApproveFullMatches)
                .NotNull().WithMessage("You need to tell us if you want to approve 100% match or delay");
        }
    }
}
