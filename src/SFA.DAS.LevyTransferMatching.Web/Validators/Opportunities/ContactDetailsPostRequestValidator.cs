using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    public class ContactDetailsPostRequestValidator : AbstractValidator<ContactDetailsPostRequest>
    {
        private const string EmailAddressError = "Enter your email address";

        public ContactDetailsPostRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Enter your first name");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Enter your last name");

            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage(EmailAddressError);

            When(
                x => !string.IsNullOrEmpty(x.EmailAddress),
                () =>
                {
                    RuleFor(x => x.EmailAddress)
                        .EmailAddress()
                        .WithMessage(EmailAddressError);
                });

            RuleForEach(x => x.AdditionalEmailAddresses)
                .EmailAddress()
                .WithMessage(EmailAddressError);

            // Check for uniqueness
            RuleFor(x => x.AdditionalEmailAddresses)
                .Must(ValidateAddressUniqueness)
                .WithMessage("You have already entered this email address");
        }

        private bool ValidateAddressUniqueness(ContactDetailsPostRequest contactDetailsPostRequest, string[] additionalEmailAddresses)
        {
            var allEmailAddresses = additionalEmailAddresses.Concat(new string[] { contactDetailsPostRequest.EmailAddress });

            var duplicatesExist = allEmailAddresses.Where(x => !string.IsNullOrWhiteSpace(x)).GroupBy(x => x).Any(x => x.Count() > 1);

            return !duplicatesExist;
        }
    }
}