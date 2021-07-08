using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    public class ContactDetailsPostRequestValidator : AbstractValidator<ContactDetailsPostRequest>
    {
        public ContactDetailsPostRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Enter your first name");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Enter your last name");

            // RuleFor(x => x.EmailAddresses[0])
            //     .NotEmpty()
            //     .WithMessage("Enter your email address");
        }
    }
}