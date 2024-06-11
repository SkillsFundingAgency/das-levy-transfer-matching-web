using FluentValidation.TestHelper;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators;

[TestFixture]
public class AmountPostModelValidatorTests
{
    private AmountPostModelValidator _amountPostModelValidator;

    [SetUp]
    public void SetUp()
    {
        _amountPostModelValidator = new AmountPostModelValidator();
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
        var amountViewModel = new AmountViewModel()
        {
            Amount = amount,
            RemainingTransferAllowance = "6,000"
        };

        //Act
        var result = _amountPostModelValidator.TestValidate(amountViewModel);

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
        var amountPostRequest = new AmountPostRequest()
        {
            Amount = amount,
            RemainingTransferAllowance = "6,000"
        };

        //Act
        var result = _amountPostModelValidator.TestValidate(amountPostRequest);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Test]
    public void Validator_Returns_Expected_Error_For_Zero_RemainingTransferAllowance()
    {
        //Arrange
        AmountViewModel amountViewModel = new AmountViewModel()
        {
            Amount = "3,000",
            RemainingTransferAllowance = "0"
        };

        //Act
        var result = _amountPostModelValidator.TestValidate(amountViewModel);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.RemainingTransferAllowance)
            .WithErrorMessage("You do not currently have any funds available to pledge");
    }

    [TestCase("2999")]
    public void Validator_Returns_Expected_Errors_For_Insufficient_RemainingTransferAllowance(string remainingTransferAllowance)
    {
        //Arrange
        AmountViewModel amountViewModel = new AmountViewModel()
        {
            Amount = "3,000",
            RemainingTransferAllowance = remainingTransferAllowance
        };

        //Act
        var result = _amountPostModelValidator.TestValidate(amountViewModel);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage($"You need to enter an amount greater than £2,000 and less than £{remainingTransferAllowance}");
    }

    [TestCase("3,000")]
    [TestCase("6000")]
    public void Validator_Returns_No_Errors_For_Valid_RemainingTransferAllowance(string remainingTransferAllowance)
    {
        //Arrange
        AmountPostRequest amountPostRequest = new AmountPostRequest()
        {
            Amount = "3,000",
            RemainingTransferAllowance = remainingTransferAllowance
        };

        //Act
        var result = _amountPostModelValidator.TestValidate(amountPostRequest);

        //Assert
        result.ShouldNotHaveValidationErrorFor(x => x.RemainingTransferAllowance);
    }
}