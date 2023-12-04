using FluentValidation.TestHelper;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators;

[TestFixture]
public class AutoApprovePostRequestValidatorTests
{
    private AutoApprovePostRequestValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new AutoApprovePostRequestValidator();
    }

    [Test]
    public void Validator_Returns_Expected_Errors_For_Invalid_Response()
    {
        //Arrange
        var postRequest = new AutoApprovePostRequest
        {
            AutomaticApprovalOption = AutomaticApprovalOption.NotApplicable
        };

        //Act
        var result = _validator.TestValidate(postRequest);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.AutomaticApprovalOption)
            .WithErrorMessage("You need to tell us if you want to approve 100% match or delay");
    }

    [TestCase(AutomaticApprovalOption.ImmediateAutoApproval)]
    [TestCase(AutomaticApprovalOption.DelayedAutoApproval)]
    public void Validator_Returns_No_Errors_For_Valid_Response(AutomaticApprovalOption option)
    {
        //Arrange
        var postRequest = new AutoApprovePostRequest()
        {
            AutomaticApprovalOption = option
        };

        //Act
        var result = _validator.TestValidate(postRequest);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.AutomaticApprovalOption);
    }

}