using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions
{
    public class IntExtensionsTests
    {
        [TestCase(100, "green")]
        [TestCase(75, "yellow")]
        [TestCase(74, "red")]
        [TestCase(0, "red")]
        public void MatchPercentageCssClass_GivenInput_ReturnsExpectedOutput(int matchPercentage, string cssClass)
        {
            var actual = matchPercentage.MatchPercentageCssClass();

            Assert.AreEqual(cssClass, actual);
        }
    }
}
