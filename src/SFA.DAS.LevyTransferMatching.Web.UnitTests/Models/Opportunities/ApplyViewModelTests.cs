using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Models.Opportunities;

public class ApplyViewModelTests
{
    private const string IncompleteString = "-";
    private readonly Fixture _fixture = new();

    [Test]
    public void ApplyViewModel_IsApprenticeshipTrainingSectionComplete_Returns_True_When_All_Fields_Entered()
    {
        var model = _fixture.Create<ApplyViewModel>();

        model.IsApprenticeshipTrainingSectionComplete.Should().BeTrue();
    }

    [Test]
    public void ApplyViewModel_IsApprenticeshipTrainingSectionComplete_Returns_False_When_One_Field_Not_Entered()
    {
        var model = _fixture.Create<ApplyViewModel>();
        model.HaveTrainingProvider = IncompleteString;

        model.IsApprenticeshipTrainingSectionComplete.Should().BeFalse();
    }

    [Test]
    public void ApplyViewModel_IsBusinessDetailsSectionComplete_Returns_True_When_All_Fields_Entered()
    {
        var model = _fixture.Create<ApplyViewModel>();

        model.IsBusinessDetailsSectionComplete.Should().BeTrue();
    }

    [Test]
    public void ApplyViewModel_IsBusinessDetailsSectionComplete_Returns_False_When_One_Field_Not_Entered()
    {
        var model = _fixture.Create<ApplyViewModel>();
        model.Sectors = null;

        model.IsBusinessDetailsSectionComplete.Should().BeFalse();
    }

    [Test]
    public void ApplyViewModel_IsContactDetailsSectionComplete_Returns_True_When_All_Fields_Entered()
    {
        var model = _fixture.Create<ApplyViewModel>();

        model.IsContactDetailsSectionComplete.Should().BeTrue();
    }

    [Test]
    public void ApplyViewModel_IsContactDetailsSectionComplete_Returns_False_When_One_Field_Not_Entered()
    {
        var model = _fixture.Create<ApplyViewModel>();
        model.ContactName = IncompleteString;

        model.IsContactDetailsSectionComplete.Should().BeFalse();
    }

    [Test]
    public void ApplyViewModel_IsComplete_Returns_True_When_All_Other_Sections_Complete()
    {
        var model = _fixture.Create<ApplyViewModel>();

        model.IsComplete.Should().BeTrue();
    }

    [Test]
    public void ApplyViewModel_IsComplete_Returns_False_When_Training_Sections_Incomplete()
    {
        var model = _fixture.Create<ApplyViewModel>();
        model.JobRole = IncompleteString;

        model.IsComplete.Should().BeFalse();
    }

    [Test]
    public void ApplyViewModel_IsComplete_Returns_False_When_BusinessDetails_Sections_Incomplete()
    {
        var model = _fixture.Create<ApplyViewModel>();
        model.Sectors = null;

        model.IsComplete.Should().BeFalse();
    }

    [Test]
    public void ApplyViewModel_IsComplete_Returns_False_When_ContactDetails_Sections_Incomplete()
    {
        var model = _fixture.Create<ApplyViewModel>();
        model.ContactName = IncompleteString;

        model.IsComplete.Should().BeFalse();
    }
}