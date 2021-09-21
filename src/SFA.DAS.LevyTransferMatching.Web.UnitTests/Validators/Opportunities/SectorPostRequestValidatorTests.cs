using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task Returns_Error_When_Sectors_Is_Null()
        {
            var request = new SectorPostRequest
            {
                Sectors = null
            };

            var result = await _validator.TestValidateAsync(request);

            result.ShouldHaveValidationErrorFor(x => x.Sectors);
        }

        [Test]
        public async Task Returns_Error_When_Sectors_Is_Empty()
        {
            var request = new SectorPostRequest
            {
                Sectors = new List<string>()
            };

            var result = await _validator.TestValidateAsync(request);

            result.ShouldHaveValidationErrorFor(x => x.Sectors);
        }

        [Test]
        public async Task Returns_No_Errors_When_Inputs_Are_Valid()
        {
            var request = new SectorPostRequest
            {
                Sectors = new List<string> { "Sector" }
            };

            var result = await _validator.TestValidateAsync(request);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
