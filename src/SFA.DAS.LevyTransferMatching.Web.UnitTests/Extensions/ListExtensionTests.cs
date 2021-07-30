using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions
{
    namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Extensions
    {
        [TestFixture]
        public class ListExtensionsTests
        {
            [Test]
            public void IsComplete_Returns_True_When_Collection_Is_Not_NullOrEmpty()
            {
                Assert.IsTrue(new List<string>() { "complete" }.IsComplete());
            }

            [Test]
            public void IsComplete_Returns_False_When_Collection_Is_Empty()
            {
                Assert.IsFalse(new List<string>().IsComplete());
            }
        }
    }
}
