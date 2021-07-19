using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Opportunities
{
    [TestFixture]
    public class SectorPostRequestValidatorTests
    {
        private SectorPostRequestValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new SectorPostRequestValidator();
        }

        [Test]
        public void Returns_Error_When_Sectors_Is_Null()
        {
            var request = new SectorPostRequest
            {
                Sectors = null,
                Postcode = "ST4 5NQ"
            };

            var result = _validator.Validate(request);

            Assert.That(!result.IsValid);
            Assert.That(result.Errors != null);
            Assert.AreEqual(result.Errors.First().PropertyName, "Sectors");
        }

        [Test]
        public void Returns_Error_When_Sectors_Is_Empty()
        {
            var request = new SectorPostRequest
            {
                Sectors = new List<string>(),
                Postcode = "ST4 5NQ"
            };

            var result = _validator.Validate(request);

            Assert.That(!result.IsValid);
            Assert.That(result.Errors != null);
            Assert.AreEqual(result.Errors.First().PropertyName, "Sectors");
        }

        [TestCase("")]
        [TestCase("invalid")]
        [TestCase(null)]
        public void Returns_Error_When_Postcode_Is_Invalid(string postcode)
        {
            var request = new SectorPostRequest
            {
                Sectors = new List<string> { "Sector" },
                Postcode = ""
            };

            var result = _validator.Validate(request);

            Assert.That(!result.IsValid);
            Assert.That(result.Errors != null);
            Assert.AreEqual(result.Errors.First().PropertyName, "Postcode");
        }

        public void Returns_No_Errors_When_Inputs_Are_Valid()
        {
            var request = new SectorPostRequest
            {
                Sectors = new List<string> { "Sector" },
                Postcode = ""
            };

            var result = _validator.Validate(request);

            Assert.That(result.IsValid);
        }
    }
}
