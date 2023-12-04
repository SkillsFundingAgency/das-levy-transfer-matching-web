using FluentValidation.TestHelper;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators;

[TestFixture]
public class AmountPostModelValidatorTests
{
    private AmountPostModelValidator amountPostModelValidator;

    [SetUp]
    public void SetUp()
    {
        amountPostModelValidator = new AmountPostModelValidator();
    }

    [TestCase("test")]
    [TestCase("")]
    [TestCase(null)]
    [TestCase("0")]
    [TestCase("1999")]
    [TestCase("6001")]
    public void Validator_Returns_Expected_Errors_For_Invalid_Amount(string amount)
    {
        //Arrange
        AmountViewModel amountViewModel = new AmountViewModel()
        {
            Amount = amount,
            RemainingTransferAllowance = "6,000"
        };

        //Act
        var result = amountPostModelValidator.TestValidate(amountViewModel);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("You need to enter an amount greater than £2,000 and less than £6,000");
    }

    [TestCase("2,000")]
    [TestCase("3,000")]
    [TestCase("6000")]
    public void Validator_Returns_No_Errors_For_Valid_Amount(string amount)
    {
        //Arrange
        AmountPostRequest amountPostRequest = new AmountPostRequest()
        {
            Amount = amount,
            RemainingTransferAllowance = "6,000"
        };

        //Act
        var result = amountPostModelValidator.TestValidate(amountPostRequest);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

}