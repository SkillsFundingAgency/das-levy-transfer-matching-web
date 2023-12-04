using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions
{
    public class DoubleExtensionsTest
    {
        [Test]
        public void ToNearest_Accuratley_Rounds_Up_OnMidPoint()
        {
            Assert.That(4_650d.ToNearest(100), Is.EqualTo(4700));
        }

        [Test]
        public void ToNearest_Accuratley_Rounds_Down()
        {
            Assert.That(4_649d.ToNearest(100), Is.EqualTo(4600));
        }

        [Test]
        public void ToNearest_Accuratley_Rounds_Up()
        {
            Assert.That(4_651d.ToNearest(100), Is.EqualTo(4700));
        }
    }
}
