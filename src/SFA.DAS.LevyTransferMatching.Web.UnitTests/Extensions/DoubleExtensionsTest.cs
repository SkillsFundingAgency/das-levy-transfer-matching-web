using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions;

public class DoubleExtensionsTest
{
    [Test]
    public void ToNearest_Accurately_Rounds_Up_OnMidPoint()
    {
        4_650d.ToNearest(100).Should().Be(4700);
    }

    [Test]
    public void ToNearest_Accurately_Rounds_Down()
    {
        4_649d.ToNearest(100).Should().Be(4600);
    }

    [Test]
    public void ToNearest_Accurately_Rounds_Up()
    {
        4_651d.ToNearest(100).Should().Be(4700);
    }
}