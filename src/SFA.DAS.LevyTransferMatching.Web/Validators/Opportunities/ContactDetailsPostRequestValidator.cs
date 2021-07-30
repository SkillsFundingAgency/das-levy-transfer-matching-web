using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System;
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

        private bool ValidateAddressUniqueness(ContactDetailsPostRequest contactDetailsPostRequest, string additionalEmailAddress, ValidationContext<ContactDetailsPostRequest> validationContext)
        {
            // First off, empty is valid.
            if (string.IsNullOrWhiteSpace(additionalEmailAddress))
            {
                return true;
            }

            // Second, if the primary email address has been specified as
            // an additional email address, simply return false - this is an
            // easy one.
            if (additionalEmailAddress == contactDetailsPostRequest.EmailAddress)
            {
                return false;
            }

            // Thirdly, it's not enough just to check for a duplicate.
            // We also need to make sure that this isn't the *first*
            // occurance -
            // as we *only* want to trigger the validation for *subsequent*
            // additional email addresses.
            var allEmailAddresses = contactDetailsPostRequest.AdditionalEmailAddresses
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            int emailIndex = (int)validationContext.MessageFormatter.PlaceholderValues["CollectionIndex"];

            var preceeding = allEmailAddresses.Take(emailIndex);

            var duplicatesExistForAdditionalEmail = preceeding.Contains(additionalEmailAddress);

            return !duplicatesExistForAdditionalEmail;
        }
    }
}