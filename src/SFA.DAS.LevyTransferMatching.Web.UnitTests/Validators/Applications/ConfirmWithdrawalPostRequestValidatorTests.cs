using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Validators.Applications;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Applications
{
    [TestFixture]
    public class ConfirmWithdrawalPostRequestValidatorTests
    {
        private Fixture _fixture;
        private ConfirmWithdrawalPostRequestValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _validator = new ConfirmWithdrawalPostRequestValidator();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReturnsValidWhenConfirmWithdrawalIsNotNull(bool confirmWithdrawal)
        {
            var request = _fixture.Build<ConfirmWithdrawalPostRequest>().With(x => x.HasConfirmed, confirmWithdrawal).Create();

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveValidationErrorFor(x => x.HasConfirmed);
        }

        [Test]
        public void ReturnsInvalidWhenConfirmWithdrawalIsNull()
        {
            var request = _fixture.Build<ConfirmWithdrawalPostRequest>().Without(x => x.HasConfirmed).Create();

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.HasConfirmed);
        }
    }
}
