using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System.Globalization;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{
    public class AmountPostModelValidator : AbstractValidator<AmountPostRequest>
    {
        public AmountPostModelValidator()
        {
            Transform(x => x.Amount, StringToNullableInt)
                .NotNull().WithMessage(m => $"Enter a number between 1 and { m.RemainingTransferAllowance }")
                .GreaterThan(0).WithMessage(m => $"Enter a number between 1 and { m.RemainingTransferAllowance }")
                .LessThanOrEqualTo(m => int.Parse(m.RemainingTransferAllowance, NumberStyles.AllowThousands)).WithMessage(m => $"Enter a number between 1 and { m.RemainingTransferAllowance }");

            RuleFor(x => x.IsNamePublic)
                .NotNull().WithMessage("Tell us whether you want to remain anonymous");
        }

        int? StringToNullableInt(string value) => int.TryParse(value, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int val) ? (int?)val : null;
    }
}
