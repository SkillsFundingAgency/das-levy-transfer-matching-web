using FluentValidation.TestHelper;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Validators.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Validators.Pledges;

public class LocationSelectPostRequestValidatorTests
{
    private LocationSelectPostRequestValidator _locationSelectPostRequestValidator;
    private Fixture _fixture;

    [SetUp]
    public void Arrange()
    {
        _locationSelectPostRequestValidator = new LocationSelectPostRequestValidator();
        _fixture = new Fixture();
    }

    [Test]
    public void Validator_Returns_No_Errors_When_All_SelectedValue_Properties_Are_Populated()
    {
        var request = _fixture.Create<LocationSelectPostRequest>();

        var result = _locationSelectPostRequestValidator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.SelectValidLocationGroups);
    }

    [Test]
    public void Validator_Returns_Error_When_A_SelectedValue_Property_Is_Null()
    {
        var request = _fixture.Create<LocationSelectPostRequest>();

        request.SelectValidLocationGroups[1].SelectedValue = null;

        var result = _locationSelectPostRequestValidator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SelectValidLocationGroups).WithErrorMessage("Please select a location");
    }
}