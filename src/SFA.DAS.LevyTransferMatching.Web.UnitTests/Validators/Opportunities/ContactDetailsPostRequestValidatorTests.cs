using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Opportunities
{
    [TestFixture]
    public class ContactDetailsPostRequestValidatorTests
    {
        private ContactDetailsPostRequestValidator _contactDetailsPostRequestValidator;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _contactDetailsPostRequestValidator = new ContactDetailsPostRequestValidator();
            _fixture = new Fixture();
        }

        [Test]
        public void Validator_Returns_Expected_Errors_For_Empty_Values()
        {
            // Arrange
            ContactDetailsPostRequest contactDetailsPostRequest = _fixture
                .Build<ContactDetailsPostRequest>()
                .With(x => x.FirstName, string.Empty)
                .With(x => x.LastName, string.Empty)
                .With(x => x.EmailAddress, string.Empty)
                .Create();

            // Act
            var result = _contactDetailsPostRequestValidator.TestValidate(contactDetailsPostRequest);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName).WithErrorMessage("Enter your first name");
            result.ShouldHaveValidationErrorFor(x => x.LastName).WithErrorMessage("Enter your last name");
            result.ShouldHaveValidationErrorFor(x => x.EmailAddress).WithErrorMessage("Enter your email address");
        }

        [Test]
        public void Validator_Returns_Expected_Error_For_Badly_Formed_EmailAddress()
        {
            // Arrange
            string emailAddress = "aaa";

            ContactDetailsPostRequest contactDetailsPostRequest = _fixture
                .Build<ContactDetailsPostRequest>()
                .With(x => x.EmailAddress, emailAddress)
                .Create();

            // Act
            var result = _contactDetailsPostRequestValidator.TestValidate(contactDetailsPostRequest);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EmailAddress).WithErrorMessage("Enter your email address");
        }

        [Test]
        public void Validator_Returns_Expected_Error_For_Badly_Formed_AdditionalEmailAddress()
        {
            // Arrange
            string emailAddress = "dd@ee";
            string[] additionalEmailAddresses = new string[]
            {
                null,
                "aaa",
                null,
                "bbb@ccc",
            };

            ContactDetailsPostRequest contactDetailsPostRequest = _fixture
                .Build<ContactDetailsPostRequest>()
                .With(x => x.EmailAddress, emailAddress)
                .With(x => x.AdditionalEmailAddresses, additionalEmailAddresses)
                .Create();

            // Act
            var results = _contactDetailsPostRequestValidator.TestValidate(contactDetailsPostRequest);

            // Assert
            results.ShouldHaveValidationErrorFor(x => x.AdditionalEmailAddresses).WithErrorMessage("Enter your email address");
        }

        [Test]
        public void Validator_Returns_No_Errors_For_NonNull_Valid_Values()
        {
            // Arrange
            string emailAddress = "aa@bb";
            string[] additionalEmailAddresses = new string[]
            {
                "cc@dd",
                null,
                null,
                null,
            };

            ContactDetailsPostRequest contactDetailsPostRequest = _fixture
                .Build<ContactDetailsPostRequest>()
                .With(x => x.EmailAddress, emailAddress)
                .With(x => x.AdditionalEmailAddresses, additionalEmailAddresses)
                .Create();

            // Act
            var results = _contactDetailsPostRequestValidator.TestValidate(contactDetailsPostRequest);

            // Assert
            results.ShouldNotHaveAnyValidationErrors();
        }
    }
}