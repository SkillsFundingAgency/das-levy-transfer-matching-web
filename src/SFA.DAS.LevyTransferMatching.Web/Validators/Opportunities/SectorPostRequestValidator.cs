using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    public class SectorPostRequestValidator : AbstractValidator<SectorPostRequest>
    {
        public SectorPostRequestValidator()
        {
            RuleFor(x => x.Sectors)
                .NotNull().WithMessage("Select one or more business sectors to describe your business")
                .NotEmpty().WithMessage("Select one or more business sectors to describe your business");

            RuleFor(x => x.Postcode)
                .NotNull().WithMessage("Enter a postcode")
                .NotEmpty().WithMessage("Enter a postcode")
                .Matches(@"^[A-Za-z]{1,2}\d[A-Za-z\d]?\s*\d[A-Za-z]{2}$").WithMessage("Enter a valid postcode");
        }
    }
}
