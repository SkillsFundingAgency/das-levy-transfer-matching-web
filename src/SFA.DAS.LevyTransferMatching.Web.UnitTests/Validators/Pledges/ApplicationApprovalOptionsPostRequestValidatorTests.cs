using FluentValidation.TestHelper;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Pledges;

[TestFixture]
public class ApplicationApprovalOptionsPostRequestValidatorTests
{
    private ApplicationApprovalOptionsPostRequestValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new ApplicationApprovalOptionsPostRequestValidator();
    }

    [Test]
    public void Validator_Returns_Error_When_AutomaticApproval_Is_Null()
    {
        //Arrange
        var request = new ApplicationApprovalOptionsPostRequest { AutomaticApproval = null };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.AutomaticApproval)
            .WithErrorMessage("You must choose to either 'Check and review apprentice details' or 'Automatically approve the apprentice details'");
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Validator_Returns_No_Error_When_AutomaticApproval_Has_Value(bool automaticApproval)
    {
        //Arrange
        var request = new ApplicationApprovalOptionsPostRequest { AutomaticApproval = automaticApproval };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.AutomaticApproval);
    }
}