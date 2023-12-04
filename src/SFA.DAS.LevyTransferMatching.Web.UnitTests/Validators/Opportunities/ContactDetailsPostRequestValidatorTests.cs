using FluentValidation.TestHelper;
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
        string emailAddress = "aa@bb";
        string[] additionalEmailAddresses = new string[]
        {
            "cc@dd", // 0
            null,    // 1
            "aa@bb", // 2
            "aa@bb", // 3
        };

        ContactDetailsPostRequest contactDetailsPostRequest = _fixture
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
        Assert.That(index2Error, Is.Not.Null);

        var index3Error = results.Errors.SingleOrDefault(x => x.PropertyName == "AdditionalEmailAddresses[3]");
        Assert.That(index3Error, Is.Not.Null);
    }

    [Test]
    public void Validator_Returns_Expected_Error_For_Duplicate_Of_Other_Additional_Email_Address()
    {
        // Arrange
        string emailAddress = "aa@bb";
        string[] additionalEmailAddresses = new string[]
        {
            "cc@dd", // 0
            "cc@dd", // 1
            null,    // 2
            "cc@dd", // 3
        };

        ContactDetailsPostRequest contactDetailsPostRequest = _fixture
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
        Assert.That(index1Error, Is.Not.Null);

        var index3Error = results.Errors.SingleOrDefault(x => x.PropertyName == "AdditionalEmailAddresses[3]");
        Assert.That(index3Error, Is.Not.Null);

        var index0Error = results.Errors.SingleOrDefault(x => x.PropertyName == "AdditionalEmailAddresses[0]");
        Assert.That(index0Error, Is.Null);
    }

    [Test]
    public void Validator_Returns_Expected_Errors_For_Strings_Over_Maximum_Lengths()
    {
        // Arrange
        string emailAddress = "this.is.a.very.very.very.very.very.long@email.address.example.com"; 
        string[] additionalEmailAddresses = new string[]
        {
            null,
            null,
            "this.is.also.a.very.very.very.very.very.long@email.address.example.com",
            null,
        };

        ContactDetailsPostRequest contactDetailsPostRequest = _fixture
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
        results.ShouldHaveValidationErrorFor(x => x.EmailAddress).WithErrorMessage("Enter your email address");
        results.ShouldHaveValidationErrorFor(x => x.AdditionalEmailAddresses).WithErrorMessage("Enter your email address");
        results.ShouldHaveValidationErrorFor(x => x.BusinessWebsite);
    }
}