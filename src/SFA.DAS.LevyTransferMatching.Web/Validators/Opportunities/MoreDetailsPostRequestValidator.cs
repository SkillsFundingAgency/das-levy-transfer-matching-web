using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    public class MoreDetailsPostRequestValidator : AbstractValidator<MoreDetailsPostRequest>
    {
        private const int MaxWords = 200;

        public MoreDetailsPostRequestValidator()
        {
            RuleFor(x => x.Details)
                .Must(details => details.Replace("\r\n", " ").Split(" ").Count() <= MaxWords)
                .When(x => x.Details != null && x.Details.Length > 0)
                .WithMessage("The details entered must be less than 200 words");
        }
    }
}
