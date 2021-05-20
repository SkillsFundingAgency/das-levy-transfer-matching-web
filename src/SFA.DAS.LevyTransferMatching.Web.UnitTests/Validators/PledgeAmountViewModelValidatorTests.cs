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
    public class PledgeAmountViewModelValidatorTests
    {
        private PledgeAmountViewModelValidator _pledgeAmountViewModelValidator;

        [SetUp]
        public void SetUp()
        {
            _pledgeAmountViewModelValidator = new PledgeAmountViewModelValidator();
        }

        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("0")]
        [TestCase("5000001")]
        public void Validator_Returns_Expected_Errors_For_Invalid_PledgeAmount(string pledgeAmount)
        {
            //Arrange
            PledgeAmountViewModel pledgeAmountViewModel = new PledgeAmountViewModel()
            {
                PledgeAmount = pledgeAmount
            };

            //Act
            var result = _pledgeAmountViewModelValidator.TestValidate(pledgeAmountViewModel);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.PledgeAmount)
                .WithErrorMessage("Enter a number between 1 and 5,000,000");
        }

        [TestCase("1")]
        [TestCase("1000000")]
        [TestCase("5000000")]
        public void Validator_Returns_No_Errors_For_Valid_PledgeAmount(string pledgeAmount)
        {
            //Arrange
            PledgeAmountViewModel pledgeAmountViewModel = new PledgeAmountViewModel()
            {
                PledgeAmount = pledgeAmount
            };

            //Act
            var result = _pledgeAmountViewModelValidator.TestValidate(pledgeAmountViewModel);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.PledgeAmount);
        }

        [TestCase(null)]
        public void Validator_Returns_Expected_Errors_For_Invalid_IsNamePublic(bool? isNamePublic)
        {
            //Arrange
            PledgeAmountViewModel pledgeAmountViewModel = new PledgeAmountViewModel()
            {
                IsNamePublic = isNamePublic
            };

            //Act
            var result = _pledgeAmountViewModelValidator.TestValidate(pledgeAmountViewModel);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.IsNamePublic)
                .WithErrorMessage("Tell us whether you want to remain anonymous");
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Validator_Returns_No_Errors_For_Valid_IsNamePublic(bool? isNamePublic)
        {
            //Arrange
            PledgeAmountViewModel pledgeAmountViewModel = new PledgeAmountViewModel()
            {
                IsNamePublic = isNamePublic
            };

            //Act
            var result = _pledgeAmountViewModelValidator.TestValidate(pledgeAmountViewModel);

            //Assert
            result.ShouldNotHaveValidationErrorFor(x => x.IsNamePublic);
        }
    }
}
