using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System.Net.Mail;

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
                        .Must(ValidEmailAddress)
                        .WithMessage(EmailAddressError);
                });

            RuleForEach(x => x.AdditionalEmailAddresses)
                .Must(x => string.IsNullOrEmpty(x) || (!string.IsNullOrEmpty(x) && ValidEmailAddress(x)))
                .WithMessage(EmailAddressError);

        }

        private bool ValidEmailAddress(string address)
        {
            try
            {
                var addr = new MailAddress(address);
                return addr.Address == address;
            }
            catch
            {
                return false;
            }
        }
    }
}