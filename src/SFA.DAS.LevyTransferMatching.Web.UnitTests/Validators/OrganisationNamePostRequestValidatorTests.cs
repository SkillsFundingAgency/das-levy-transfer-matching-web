using FluentValidation.TestHelper;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators;

[TestFixture]
public class OrganisationNamePostRequestValidatorTests
{
    private OrganisationNamePostRequestValidator organisationNamePostRequestValidator;

    [SetUp]
    public void SetUp()
    {
        organisationNamePostRequestValidator = new OrganisationNamePostRequestValidator();
    }

      
    [TestCase(null)]        
    public void Validator_Returns_Expected_Errors_For_Invalid_OrganisationNameResponse(bool? isPublic)
    {
        //Arrange
        OrganisationNamePostRequest postRequest = new OrganisationNamePostRequest()
        {
            IsNamePublic = isPublic
        };

        //Act
        var result = organisationNamePostRequestValidator.TestValidate(postRequest);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.IsNamePublic)
            .WithErrorMessage("You need to tell us if you want to show or hide your organisation name");
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Validator_Returns_No_Errors_For_Valid_OrganisationNameResponse(bool? isPublic)
    {
        //Arrange
        OrganisationNamePostRequest postRequest = new OrganisationNamePostRequest()
        {
            IsNamePublic = isPublic
        };

        //Act
        var result = organisationNamePostRequestValidator.TestValidate(postRequest);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.IsNamePublic);
    }

}