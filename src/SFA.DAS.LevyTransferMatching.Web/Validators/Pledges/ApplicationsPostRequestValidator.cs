using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{  
    public class ApplicationsPostRequestValidator : AbstractValidator<ApplicationsPostRequest>
    {
        public ApplicationsPostRequestValidator()
        {
            RuleFor(x => x.ApplicationsToReject).NotNull().WithMessage("You must choose applications from the list to reject");
        }
    }
}
