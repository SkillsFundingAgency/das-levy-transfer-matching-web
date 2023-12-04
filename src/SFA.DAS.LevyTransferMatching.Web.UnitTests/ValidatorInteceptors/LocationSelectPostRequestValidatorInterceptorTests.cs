using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.ValidatorInterceptors;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.ValidatorInteceptors;

public class LocationSelectPostRequestValidatorInterceptorTests
{
    private LocationSelectPostRequestValidatorInterceptor _locationSelectPostRequestValidatorInterceptor;
    private Fixture _fixture;

    [SetUp]
    public void Arrange()
    {
        _locationSelectPostRequestValidatorInterceptor = new LocationSelectPostRequestValidatorInterceptor();
        _fixture = new Fixture();
    }

    [Test]
    public void AfterAspNetValidation_Modifies_ModelState_To_Contain_Top_Level_Property_Entries()
    {
        // Arrange
        var mockActionContext = new Mock<ActionContext>();
        var mockValidationContext = new Mock<IValidationContext>();

        var locationSelectPostRequest = _fixture.Create<LocationSelectPostRequest>();

        mockValidationContext
            .Setup(x => x.InstanceToValidate)
            .Returns(locationSelectPostRequest);

        var validationResult = _fixture.Create<FluentValidation.Results.ValidationResult>();

        // Act
        _locationSelectPostRequestValidatorInterceptor.AfterAspNetValidation(mockActionContext.Object, mockValidationContext.Object, validationResult);

        // Assert
        var expectedModelStateKeys = Enumerable.Range(0, locationSelectPostRequest.SelectValidLocationGroups.Length)
            .Select(x => $"{nameof(LocationSelectPostRequest.SelectValidLocationGroups)}[{x}]");

        var actualModelStateKeys = mockActionContext.Object.ModelState.Keys;

        foreach (var expectedKey in expectedModelStateKeys)
        {
            Assert.That(actualModelStateKeys, Has.Member(expectedKey));
        }
    }
}