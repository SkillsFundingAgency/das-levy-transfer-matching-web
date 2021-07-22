using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    public class ContactDetailsPostRequestValidator : AbstractValidator<ContactDetailsPostRequest>
    {
        private const string FirstNameError = "Enter your first name";
        private const string LastNameError = "Enter your last name";
        private const string EmailAddressError = "Enter your email address";

        public ContactDetailsPostRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(FirstNameError);

            RuleFor(x => x.FirstName)
                .MaximumLength(25)
                .WithMessage(FirstNameError);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(LastNameError);

            RuleFor(x => x.LastName)
                .MaximumLength(25)
                .WithMessage(LastNameError);

            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage(EmailAddressError);

            RuleFor(x => x.EmailAddress)
                .MaximumLength(50)
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

            RuleForEach(x => x.AdditionalEmailAddresses)
                .MaximumLength(50)
                .WithMessage(EmailAddressError);

            // Check for uniqueness
            RuleForEach(x => x.AdditionalEmailAddresses)
                .Must(ValidateAddressUniqueness)
                .WithMessage("You have already entered this email address");

            RuleFor(x => x.BusinessWebsite)
                .MaximumLength(75);
        }

        private bool ValidateAddressUniqueness(ContactDetailsPostRequest contactDetailsPostRequest, string additionalEmailAddress)
        {
            var allEmailAddresses = contactDetailsPostRequest.AdditionalEmailAddresses
                .Concat(new string[] { contactDetailsPostRequest.EmailAddress })
                .Where(x => !string.IsNullOrWhiteSpace(x));

            var duplicatesExistForAdditionalEmail = allEmailAddresses
                .GroupBy(x => x)
                .Any(x => x.Count() > 1 && x.Contains(additionalEmailAddress));

            return !duplicatesExistForAdditionalEmail;
        }
    }
}