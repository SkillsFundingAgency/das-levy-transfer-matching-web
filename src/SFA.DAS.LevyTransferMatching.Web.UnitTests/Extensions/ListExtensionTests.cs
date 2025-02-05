using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions;

[TestFixture]
public class ListExtensionsTests
{
    [Test]
    public void IsComplete_Returns_True_When_Collection_Is_Not_NullOrEmpty()
    {
        new List<string> { "complete" }.IsComplete().Should().BeTrue();
    }

    [Test]
    public void IsComplete_Returns_False_When_Collection_Is_Empty()
    {
        new List<string>().IsComplete().Should().BeFalse();
    }
}