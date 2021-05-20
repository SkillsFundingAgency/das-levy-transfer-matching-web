using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{
    public class PledgeAmountViewModelValidator : AbstractValidator<PledgeAmountViewModel>
    {
        public PledgeAmountViewModelValidator()
        {
            Transform(x => x.PledgeAmount, StringToNullableInt)
                .NotNull().WithMessage("Enter a number between 1 and 5,000,000")
                .GreaterThan(0).WithMessage("Enter a number between 1 and 5,000,000")
                .LessThanOrEqualTo(5000000).WithMessage("Enter a number between 1 and 5,000,000");

            RuleFor(x => x.IsNamePublic)
                .NotNull().WithMessage("Tell us whether you want to remain anonymous");
        }

        int? StringToNullableInt(string value) => int.TryParse(value, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int val) ? (int?)val : null;
    }
}
