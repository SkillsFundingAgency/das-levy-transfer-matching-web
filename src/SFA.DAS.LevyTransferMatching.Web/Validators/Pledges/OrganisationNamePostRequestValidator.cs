using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{
    public class OrganisationNamePostRequestValidator : AbstractValidator<OrganisationNamePostRequest>
    {
        public OrganisationNamePostRequestValidator()
        {           
            RuleFor(x => x.IsNamePublic)
                .NotNull().WithMessage("You need to tell us if you want to show or hide your organisation name");
        }
    }
}
