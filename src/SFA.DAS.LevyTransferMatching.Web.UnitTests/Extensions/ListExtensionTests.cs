using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions;

[TestFixture]
public class ListExtensionsTests
{
    [Test]
    public void IsComplete_Returns_True_When_Collection_Is_Not_NullOrEmpty()
    {
        Assert.That(new List<string> { "complete" }.IsComplete(), Is.True);
    }

    [Test]
    public void IsComplete_Returns_False_When_Collection_Is_Empty()
    {
        Assert.That(new List<string>().IsComplete(), Is.False);
    }
}