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
            RuleForEach(x => x.AdditionalEmailAddresses)
                .Must(ValidateAddressUniqueness)
                .WithMessage("You have already entered this email address");
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