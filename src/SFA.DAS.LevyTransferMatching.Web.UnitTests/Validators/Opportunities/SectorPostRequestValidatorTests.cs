using FluentValidation.TestHelper;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Opportunities;

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
    public void Returns_No_Errors_When_Inputs_Are_Valid()
    {
        var request = new SectorPostRequest
        {
            Sectors = new List<string> { "Sector" },
            HasPledgeLocations = true,
            Locations = new List<int> { 1 }
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Returns_Errors_When_Additional_Location_Is_Selected_But_Additional_Location_Text_Is_Not_Provided()
    {
        var request = new SectorPostRequest
        {
            Sectors = new List<string> { "Sector" },
            HasPledgeLocations = true,
            AdditionalLocation = true,
            AdditionalLocationText = string.Empty
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AdditionalLocationText);
    }

    [Test]
    public void Returns_No_Error_When_Additional_Location_Is_Selected_And_Additional_Location_Text_Is_Provided()
    {
        var request = new SectorPostRequest
        {
            Sectors = new List<string> { "Sector" },
            HasPledgeLocations = true,
            AdditionalLocation = true,
            AdditionalLocationText = "Test additional location"
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Returns_Error_When_No_Pledge_Locations_And_Specific_Location_Is_Not_Provided()
    {
        var request = new SectorPostRequest
        {
            Sectors = new List<string> { "Sector" },
            HasPledgeLocations = false,
            SpecificLocation = string.Empty
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SpecificLocation);
    }

    [Test]
    public void Returns_No_Error_When_No_Pledge_Locations_And_Specific_Location_Is_Provided()
    {
        var request = new SectorPostRequest
        {
            Sectors = new List<string> { "Sector" },
            HasPledgeLocations = false,
            SpecificLocation = "Test location"
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}