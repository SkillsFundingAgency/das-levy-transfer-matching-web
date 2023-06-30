using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System.Globalization;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{
    public class OrganisationNamePostRequestValidator : AbstractValidator<OrganisationNamePostRequest>
    {
        public OrganisationNamePostRequestValidator()
        {           
            RuleFor(x => x.IsNamePublic)
                .NotNull().WithMessage("You need to select whether or not you want your pledge to show your organisation’s name publicly");
        }
    }
}
