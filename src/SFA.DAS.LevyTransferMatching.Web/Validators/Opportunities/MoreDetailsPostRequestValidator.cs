using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;

public class MoreDetailsPostRequestValidator : AbstractValidator<MoreDetailsPostRequest>
{
    private const int MaxWords = 200;
    private const string Space = " ";

    public MoreDetailsPostRequestValidator()
    {
        RuleFor(x => x.Details)
            .Must(details => details.Replace("\r\n", Space).Split(Space).Count(x => !string.IsNullOrEmpty(x)) <= MaxWords)
            .When(x => !string.IsNullOrEmpty(x.Details))
            .WithMessage("The details entered must be less than 200 words");
    }
}