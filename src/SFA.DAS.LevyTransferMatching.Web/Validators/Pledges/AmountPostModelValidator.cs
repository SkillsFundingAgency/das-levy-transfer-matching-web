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
                .NotNull().WithMessage(m => $"You need to enter an amount greater than £2,000 and less than £{ m.RemainingTransferAllowance }")
                .GreaterThanOrEqualTo(2000).WithMessage(m => $"You need to enter an amount greater than £2,000 and less than £{ m.RemainingTransferAllowance }")
                .LessThanOrEqualTo(m => int.Parse(m.RemainingTransferAllowance, NumberStyles.AllowThousands)).WithMessage(m => $"You need to enter an amount greater than £2,000 and less than £{ m.RemainingTransferAllowance }");

        }

        int? StringToNullableInt(string value) => int.TryParse(value, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out int val) ? (int?)val : null;
    }
}
