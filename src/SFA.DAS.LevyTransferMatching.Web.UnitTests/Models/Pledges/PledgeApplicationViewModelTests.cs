using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Models.Pledges
{
    public class PledgeApplicationViewModelTests
    {
        private const string LOCATION = "location";

        private readonly Fixture _fixture = new Fixture();
        [Test]
        public void MatchPercentage_WhenGivenAllMatchingInputsThen_Returns100Percent()
        {
            var viewModel = _fixture.Create<PledgeApplicationViewModel>();
            viewModel.Location = LOCATION;
            viewModel.PledgeLocations = viewModel.PledgeLocations.Append(LOCATION);

            viewModel.MatchPercentage().ShouldCompare("100%");
        }
    }
}
