using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Pledges;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators
{
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
        [TestCase("5000001")]
        public void Validator_Returns_Expected_Errors_For_Invalid_Amount(string amount)
        {
            //Arrange
            AmountViewRequest amountViewRequest = new AmountViewRequest()
            {
                Amount = amount
            };

            //Act
            var result = amountPostModelValidator.TestValidate(amountViewRequest);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.Amount)
                .WithErrorMessage("Enter a number between 1 and 5,000,000");
        }

        [TestCase("1")]
        [TestCase("1000000")]
        [TestCase("5000000")]
        public void Validator_Returns_No_Errors_For_Valid_Amount(string amount)
        {
            //Arrange
            AmountPostRequest amountPostRequest = new AmountPostRequest()
            {
                Amount = amount
            };

            //Act
            var result = amountPostModelValidator.TestValidate(amountPostRequest);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Amount);
        }

        [TestCase(null)]
        public void Validator_Returns_Expected_Errors_For_Invalid_IsNamePublic(bool? isNamePublic)
        {
            //Arrange
            AmountPostRequest amountPostRequest = new AmountPostRequest()
            {
                IsNamePublic = isNamePublic
            };

            //Act
            var result = amountPostModelValidator.TestValidate(amountPostRequest);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.IsNamePublic)
                .WithErrorMessage("Tell us whether you want to remain anonymous");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Validator_Returns_No_Errors_For_Valid_IsNamePublic(bool? isNamePublic)
        {
            //Arrange
            AmountPostRequest amountPostRequest = new AmountPostRequest()
            {
                IsNamePublic = isNamePublic
            };

            //Act
            var result = amountPostModelValidator.TestValidate(amountPostRequest);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.IsNamePublic);
        }
    }
}
