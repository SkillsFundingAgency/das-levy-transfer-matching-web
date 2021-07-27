using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Validators;
using SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Opportunities
{
    [TestFixture]
    public class SectorPostRequestValidatorTests
    {
        private AsyncValidator<SectorPostRequest> _validator;

        private Mock<IOpportunitiesService> _opportunitiesService;

        private string validPostcode;
        private string nonExistentPostcode;

        [SetUp]
        public void SetUp()
        {
            validPostcode = "ST4 5NQ";
            nonExistentPostcode = "NotFoundPostcode";

            _opportunitiesService = new Mock<IOpportunitiesService>();
            _opportunitiesService.Setup(x => x.GetSector(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new GetSectorResponse { Location = "Valid Location" });
            _opportunitiesService.Setup(x => x.GetSector(It.IsAny<long>(), It.IsAny<int>(), nonExistentPostcode)).ReturnsAsync(new GetSectorResponse { Location = null });

            _validator = new SectorPostRequestAsyncValidator(_opportunitiesService.Object);
        }

        [Test]
        public async Task Returns_Error_When_Sectors_Is_Null()
        {
            var request = new SectorPostRequest
            {
                Sectors = null,
                Postcode = validPostcode
            };

            var result = await _validator.TestValidateAsync(request);

            result.ShouldHaveValidationErrorFor(x => x.Sectors);
        }

        [Test]
        public async Task Returns_Error_When_Sectors_Is_Empty()
        {
            var request = new SectorPostRequest
            {
                Sectors = new List<string>(),
                Postcode = validPostcode
            };

            var result = await _validator.TestValidateAsync(request);

            result.ShouldHaveValidationErrorFor(x => x.Sectors);
        }

        [TestCase("")]
        [TestCase("RegexInvalid")]
        [TestCase(null)]
        public async Task Returns_Error_When_Postcode_Is_Invalid(string postcode)
        {
            var request = new SectorPostRequest
            {
                Sectors = new List<string> { "Sector" },
                Postcode = postcode
            };

            var result = await _validator.TestValidateAsync(request);

            result.ShouldHaveValidationErrorFor(x => x.Postcode);
        }

        [Test]
        public async Task Returns_Error_When_Postcode_Is_Not_Found()
        {
            var request = new SectorPostRequest
            {
                Sectors = new List<string> { "Sector" },
                Postcode = nonExistentPostcode
            };

            var result = await _validator.TestValidateAsync(request);

            result.ShouldHaveValidationErrorFor(x => x.Postcode);
        }

        [Test]
        public async Task Returns_No_Errors_When_Inputs_Are_Valid()
        {
            var request = new SectorPostRequest
            {
                Sectors = new List<string> { "Sector" },
                Postcode = validPostcode
            };

            var result = await _validator.TestValidateAsync(request);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
