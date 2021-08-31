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
        private const string SECTOR = "Sector";
        private const string JOBROLE = "Type ofjob role";
        private const string LEVEL = "Level7";
        private readonly Fixture _fixture = new Fixture();
        
        [Test]
        public void MatchPercentage_WhenGivenAllMatchingInputsThen_Returns100Percent()
        {
            var viewModel = _fixture.Create<ApplicationViewModel>();
            viewModel.Location = LOCATION;
            viewModel.PledgeLocations = viewModel.PledgeLocations.Append(LOCATION);
            viewModel.Sector = new List<string>
            {
                SECTOR
            };
            viewModel.PledgeSectors = viewModel.PledgeSectors.Append(SECTOR);
            viewModel.JobRole = JOBROLE;
            viewModel.PledgeJobRoles = viewModel.PledgeJobRoles.Append(JOBROLE);
            viewModel.Level = 7;
            viewModel.PledgeLevels = viewModel.PledgeLevels.Append(LEVEL);

            viewModel.MatchPercentage().ShouldCompare("100%");
            Assert.IsTrue(viewModel.JobRoleHasMatched);
            Assert.IsTrue(viewModel.LevelHasMatched);
            Assert.IsTrue(viewModel.LocationHasMatched);
            Assert.IsTrue(viewModel.SectorHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenGivenNoMatchingLocationThen_Returns75Percent()
        {
            var viewModel = _fixture.Create<ApplicationViewModel>();
            viewModel.Sector = new List<string>
            {
                SECTOR
            };
            viewModel.PledgeSectors = viewModel.PledgeSectors.Append(SECTOR);
            viewModel.JobRole = JOBROLE;
            viewModel.PledgeJobRoles = viewModel.PledgeJobRoles.Append(JOBROLE);
            viewModel.Level = 7;
            viewModel.PledgeLevels = viewModel.PledgeLevels.Append(LEVEL);

            viewModel.MatchPercentage().ShouldCompare("75%");
            Assert.IsFalse(viewModel.LocationHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenGivenNoMatchingLocationAndSectorsThen_Returns50Percent()
        {
            var viewModel = _fixture.Create<ApplicationViewModel>();
            viewModel.JobRole = JOBROLE;
            viewModel.PledgeJobRoles = viewModel.PledgeJobRoles.Append(JOBROLE);
            viewModel.Level = 7;
            viewModel.PledgeLevels = viewModel.PledgeLevels.Append(LEVEL);

            viewModel.MatchPercentage().ShouldCompare("50%");
            Assert.IsFalse(viewModel.SectorHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenGivenNoMatchingLocationAndSectorsAndJobRoleThen_Returns25Percent()
        {
            var viewModel = _fixture.Create<ApplicationViewModel>();
            viewModel.Level = 7;
            viewModel.PledgeLevels = viewModel.PledgeLevels.Append(LEVEL);

            viewModel.MatchPercentage().ShouldCompare("25%");
            Assert.IsFalse(viewModel.JobRoleHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenGivenNoMatchingLocationAndSectorsAndJobRoleAndLevelThen_Returns0Percent()
        {
            var viewModel = _fixture.Create<ApplicationViewModel>();

            viewModel.MatchPercentage().ShouldCompare("0%");
            Assert.IsFalse(viewModel.LevelHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenPledgeSectorsIsEmpty_ThenDefaultsToAMatch()
        {
            var viewModel = _fixture.Create<ApplicationViewModel>();

            viewModel.PledgeSectors = new List<string>();

            viewModel.MatchPercentage();

            Assert.IsTrue(viewModel.SectorHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenPledgeJobRolesIsEmpty_ThenDefaultsToAMatch()
        {
            var viewModel = _fixture.Create<ApplicationViewModel>();

            viewModel.PledgeJobRoles = new List<string>();

            viewModel.MatchPercentage();

            Assert.IsTrue(viewModel.JobRoleHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenPledgeLocationsIsEmpty_ThenDefaultsToAMatch()
        {
            var viewModel = _fixture.Create<ApplicationViewModel>();

            viewModel.PledgeLocations = new List<string>();

            viewModel.MatchPercentage();

            Assert.IsTrue(viewModel.LocationHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenPledgeLevelsIsEmpty_ThenDefaultsToAMatch()
        {
            var viewModel = _fixture.Create<ApplicationViewModel>();

            viewModel.PledgeLevels = new List<string>();

            viewModel.MatchPercentage();

            Assert.IsTrue(viewModel.LevelHasMatched);
        }
    }
}
