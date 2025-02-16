﻿using FluentValidation.TestHelper;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Opportunities;

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
        var contactDetailsPostRequest = _fixture
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
        result.ShouldHaveValidationErrorFor(x => x.EmailAddress).WithErrorMessage("Enter your email address in the correct format, like name@example.com");
    }

    [Test]
    public void Validator_Returns_Expected_Error_For_Badly_Formed_EmailAddress()
    {
        // Arrange
        const string emailAddress = "aaa";

        var contactDetailsPostRequest = _fixture
            .Build<ContactDetailsPostRequest>()
            .With(x => x.EmailAddress, emailAddress)
            .Create();

        // Act
        var result = _contactDetailsPostRequestValidator.TestValidate(contactDetailsPostRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmailAddress).WithErrorMessage("Enter your email address in the correct format, like name@example.com");
    }

    [Test]
    public void Validator_Returns_Expected_Error_For_Badly_Formed_AdditionalEmailAddress()
    {
        // Arrange
        const string emailAddress = "dd@ee";
        var additionalEmailAddresses = new[]
        {
            null,
            "aaa",
            null,
            "bbb@ccc",
        };

        var contactDetailsPostRequest = _fixture
            .Build<ContactDetailsPostRequest>()
            .With(x => x.EmailAddress, emailAddress)
            .With(x => x.AdditionalEmailAddresses, additionalEmailAddresses)
            .Create();

        // Act
        var results = _contactDetailsPostRequestValidator.TestValidate(contactDetailsPostRequest);

        // Assert
        results.ShouldHaveValidationErrorFor(x => x.AdditionalEmailAddresses).WithErrorMessage("Enter your email address in the correct format, like name@example.com");
    }

    [Test]
    public void Validator_Returns_No_Errors_For_NonNull_Valid_Values()
    {
        // Arrange
        const string emailAddress = "aa@bb";
        var additionalEmailAddresses = new[]
        {
            "cc@dd",
            null,
            null,
            null,
        };

        var contactDetailsPostRequest = _fixture
            .Build<ContactDetailsPostRequest>()
            .With(x => x.FirstName, new string(_fixture.CreateMany<char>(25).ToArray()))
            .With(x => x.LastName, new string(_fixture.CreateMany<char>(25).ToArray()))
            .With(x => x.EmailAddress, emailAddress)
            .With(x => x.AdditionalEmailAddresses, additionalEmailAddresses)
            .Create();

        // Act
        var results = _contactDetailsPostRequestValidator.TestValidate(contactDetailsPostRequest);

        // Assert
        results.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validator_Returns_Expected_Error_For_Duplicate_Of_Primary_Email_Address()
    {
        // Arrange
        const string emailAddress = "aa@bb";
        var additionalEmailAddresses = new[]
        {
            "cc@dd", // 0
            null,    // 1
            "aa@bb", // 2
            "aa@bb", // 3
        };

        var contactDetailsPostRequest = _fixture
            .Build<ContactDetailsPostRequest>()
            .With(x => x.EmailAddress, emailAddress)
            .With(x => x.AdditionalEmailAddresses, additionalEmailAddresses)
            .Create();

        // Act
        var results = _contactDetailsPostRequestValidator.TestValidate(contactDetailsPostRequest);

        // Assert
        results.ShouldHaveValidationErrorFor(x => x.AdditionalEmailAddresses).WithErrorMessage("You have already entered this email address");

        // Note - we should have 2 validation errors,
        //        for AdditionalEmailAddresses[2] and AdditionalEmailAddresses[3]
        var index2Error = results.Errors.SingleOrDefault(x => x.PropertyName == "AdditionalEmailAddresses[2]");
        index2Error.Should().NotBeNull();

        var index3Error = results.Errors.SingleOrDefault(x => x.PropertyName == "AdditionalEmailAddresses[3]");
        index3Error.Should().NotBeNull();
    }

    [Test]
    public void Validator_Returns_Expected_Error_For_Duplicate_Of_Other_Additional_Email_Address()
    {
        // Arrange
        const string emailAddress = "aa@bb";
        var additionalEmailAddresses = new[]
        {
            "cc@dd", // 0
            "cc@dd", // 1
            null,    // 2
            "cc@dd", // 3
        };

        var contactDetailsPostRequest = _fixture
            .Build<ContactDetailsPostRequest>()
            .With(x => x.EmailAddress, emailAddress)
            .With(x => x.AdditionalEmailAddresses, additionalEmailAddresses)
            .Create();

        // Act
        var results = _contactDetailsPostRequestValidator.TestValidate(contactDetailsPostRequest);

        // Assert
        results.ShouldHaveValidationErrorFor(x => x.AdditionalEmailAddresses).WithErrorMessage("You have already entered this email address");

        // Note - we should have 2 validation errors,
        //        for AdditionalEmailAddresses[1] and AdditionalEmailAddresses[3]
        //        But *not* for AdditionalEmailAddresses[0]!
        var index1Error = results.Errors.SingleOrDefault(x => x.PropertyName == "AdditionalEmailAddresses[1]");
        index1Error.Should().NotBeNull();

        var index3Error = results.Errors.SingleOrDefault(x => x.PropertyName == "AdditionalEmailAddresses[3]");
        index3Error.Should().NotBeNull();

        var index0Error = results.Errors.SingleOrDefault(x => x.PropertyName == "AdditionalEmailAddresses[0]");
        index0Error.Should().BeNull();
    }

    [Test]
    public void Validator_Returns_Expected_Errors_For_Strings_Over_Maximum_Lengths()
    {
        // Arrange
        const string emailAddress = "this.is.a.very.very.very.very.very.long@email.address.example.com"; 
        var additionalEmailAddresses = new[]
        {
            null,
            null,
            "this.is.also.a.very.very.very.very.very.long@email.address.example.com",
            null,
        };

        var contactDetailsPostRequest = _fixture
            .Build<ContactDetailsPostRequest>()
            .With(x => x.FirstName, new string(_fixture.CreateMany<char>(26).ToArray()))
            .With(x => x.LastName, new string(_fixture.CreateMany<char>(26).ToArray()))
            .With(x => x.EmailAddress, emailAddress)
            .With(x => x.AdditionalEmailAddresses, additionalEmailAddresses)
            .With(x => x.BusinessWebsite, new string(_fixture.CreateMany<char>(76).ToArray()))
            .Create();

        // Act
        var results = _contactDetailsPostRequestValidator.TestValidate(contactDetailsPostRequest);

        // Assert
        results.ShouldHaveValidationErrorFor(x => x.FirstName).WithErrorMessage("Enter your first name");
        results.ShouldHaveValidationErrorFor(x => x.LastName).WithErrorMessage("Enter your last name");
        results.ShouldHaveValidationErrorFor(x => x.EmailAddress).WithErrorMessage("Enter your email address in the correct format, like name@example.com");
        results.ShouldHaveValidationErrorFor(x => x.AdditionalEmailAddresses).WithErrorMessage("Enter your email address in the correct format, like name@example.com");
        results.ShouldHaveValidationErrorFor(x => x.BusinessWebsite);
    }
}