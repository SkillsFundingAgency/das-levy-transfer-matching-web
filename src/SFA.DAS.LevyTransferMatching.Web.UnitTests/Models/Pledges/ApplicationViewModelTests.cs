using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Models.Pledges
{
    public class ApplicationViewModelTests
    {
        private const string LOCATION = "location";
        private const string SECTOR = "Sector";
        private const string JOBROLE = "Type ofjob role";
        private const string LEVEL = "Level7";
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void MatchPercentage_WhenGivenAllMatchingInputsThen_Returns100Percent()
        {
            var viewModel = new GetApplicationViewModel
            {
                Sectors = new List<string> { SECTOR },
                AllSectors = new List<ReferenceDataItem>
                {
                    new ReferenceDataItem
                    {
                        Id = SECTOR,
                        Description = SECTOR,
                        ShortDescription = SECTOR
                    }
                },
                PledgeSectors = new List<string>
                {
                    SECTOR
                },
                PledgeJobRoles = new List<string>
                {
                    JOBROLE
                },
                PledgeLevels = new List<string>
                {
                    LEVEL
                },
                PledgeLocations = new List<string>
                {
                    LOCATION
                },
                Location = LOCATION,
                JobRole = JOBROLE,
                Level = 7
            };

            viewModel.SetupModel();

            viewModel.MatchPercentage.ShouldCompare("100%");
            Assert.IsTrue(viewModel.JobRoleHasMatched);
            Assert.IsTrue(viewModel.LevelHasMatched);
            Assert.IsTrue(viewModel.LocationHasMatched);
            Assert.IsTrue(viewModel.SectorHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenGivenNoMatchingLocationThen_Returns75Percent()
        {
            var viewModel = new GetApplicationViewModel
            {
                Sectors = new List<string> { SECTOR },
                AllSectors = new List<ReferenceDataItem>
                {
                    new ReferenceDataItem
                    {
                        Id = SECTOR,
                        Description = SECTOR,
                        ShortDescription = SECTOR
                    }
                },
                PledgeSectors = new List<string>
                {
                    SECTOR
                },
                PledgeJobRoles = new List<string>
                {
                    JOBROLE
                },
                PledgeLevels = new List<string>
                {
                    LEVEL
                },
                PledgeLocations = new List<string>
                {
                    "NoMatch"
                },
                Location = LOCATION,
                JobRole = JOBROLE,
                Level = 7
            };

            viewModel.SetupModel();

            viewModel.MatchPercentage.ShouldCompare("75%");
            Assert.IsFalse(viewModel.LocationHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenGivenNoMatchingLocationAndSectorsThen_Returns50Percent()
        {
            var viewModel = new GetApplicationViewModel
            {
                Sectors = new List<string> { SECTOR },
                AllSectors = new List<ReferenceDataItem>
                {
                    new ReferenceDataItem
                    {
                        Id = SECTOR,
                        Description = SECTOR,
                        ShortDescription = SECTOR
                    }
                },
                PledgeSectors = new List<string>
                {
                    "NoMatch"
                },
                PledgeJobRoles = new List<string>
                {
                    JOBROLE
                },
                PledgeLevels = new List<string>
                {
                    LEVEL
                },
                PledgeLocations = new List<string>
                {
                    "NoMatch"
                },
                Location = LOCATION,
                JobRole = JOBROLE,
                Level = 7
            };

            viewModel.SetupModel();

            viewModel.MatchPercentage.ShouldCompare("50%");
            Assert.IsFalse(viewModel.SectorHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenGivenNoMatchingLocationAndSectorsAndJobRoleThen_Returns25Percent()
        {
            var viewModel = new GetApplicationViewModel
            {
                Sectors = new List<string> { SECTOR },
                AllSectors = new List<ReferenceDataItem>
                {
                    new ReferenceDataItem
                    {
                        Id = SECTOR,
                        Description = SECTOR,
                        ShortDescription = SECTOR
                    }
                },
                PledgeSectors = new List<string>
                {
                    "NoMatch"
                },
                PledgeJobRoles = new List<string>
                {
                    "NoMatch"
                },
                PledgeLevels = new List<string>
                {
                    LEVEL
                },
                PledgeLocations = new List<string>
                {
                    "NoMatch"
                },
                Location = LOCATION,
                JobRole = JOBROLE,
                Level = 7
            };

            viewModel.SetupModel();

            viewModel.MatchPercentage.ShouldCompare("25%");
            Assert.IsFalse(viewModel.JobRoleHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenGivenNoMatchingLocationAndSectorsAndJobRoleAndLevelThen_Returns0Percent()
        {
            var viewModel = new GetApplicationViewModel();

            viewModel.MatchPercentage.ShouldCompare("0%");
            Assert.IsFalse(viewModel.LevelHasMatched);
        }

        [Test]
        public void MatchPercentage_WhenPledgeSectorsIsEmpty_ThenDefaultsToAMatch()
        {
            var viewModel = new GetApplicationViewModel
            {
                Sectors = new List<string> { SECTOR },
                AllSectors = new List<ReferenceDataItem>
                {
                    new ReferenceDataItem
                    {
                        Id = SECTOR,
                        Description = SECTOR,
                        ShortDescription = SECTOR
                    }
                },
                PledgeSectors = new List<string>(),
                PledgeJobRoles = new List<string>
                {
                    "NoMatch"
                },
                PledgeLevels = new List<string>
                {
                    "NoMatch"
                },
                PledgeLocations = new List<string>
                {
                    "NoMatch"
                },
                Location = LOCATION,
                JobRole = JOBROLE,
                Level = 7
            };

            viewModel.SetupModel();

            var matchPercentage = viewModel.MatchPercentage;

            Assert.IsTrue(viewModel.SectorHasMatched);
            Assert.AreEqual("25%", matchPercentage);
        }

        [Test]
        public void MatchPercentage_WhenPledgeJobRolesIsEmpty_ThenDefaultsToAMatch()
        {
            var viewModel = new GetApplicationViewModel
            {
                Sectors = new List<string> { SECTOR },
                AllSectors = new List<ReferenceDataItem>
                {
                    new ReferenceDataItem
                    {
                        Id = SECTOR,
                        Description = SECTOR,
                        ShortDescription = SECTOR
                    }
                },
                PledgeSectors = new List<string>
                {
                    "NoMatch"
                },
                PledgeJobRoles = new List<string>(),
                PledgeLevels = new List<string>
                {
                    "NoMatch"
                },
                PledgeLocations = new List<string>
                {
                    "NoMatch"
                },
                Location = LOCATION,
                JobRole = JOBROLE,
                Level = 7
            };

            viewModel.SetupModel();

            var matchPercentage = viewModel.MatchPercentage;

            Assert.IsTrue(viewModel.JobRoleHasMatched);
            Assert.AreEqual("25%", matchPercentage);
        }

        [Test]
        public void MatchPercentage_WhenPledgeLocationsIsEmpty_ThenDefaultsToAMatch()
        {
            var viewModel = new GetApplicationViewModel
            {
                Sectors = new List<string> { SECTOR },
                AllSectors = new List<ReferenceDataItem>
                {
                    new ReferenceDataItem
                    {
                        Id = SECTOR,
                        Description = SECTOR,
                        ShortDescription = SECTOR
                    }
                },
                PledgeSectors = new List<string>
                {
                    "NoMatch"
                },
                PledgeJobRoles = new List<string>
                {
                    "NoMatch"
                },
                PledgeLevels = new List<string>
                {
                    "NoMatch"
                },
                PledgeLocations = new List<string>(),
                Location = LOCATION,
                JobRole = JOBROLE,
                Level = 7
            };

            viewModel.SetupModel();

            var matchPercentage = viewModel.MatchPercentage;

            Assert.IsTrue(viewModel.LocationHasMatched);
            Assert.AreEqual("25%", matchPercentage);
        }

        [Test]
        public void MatchPercentage_WhenPledgeLevelsIsEmpty_ThenDefaultsToAMatch()
        {
            var viewModel = new GetApplicationViewModel
            {
                Sectors = new List<string> { SECTOR },
                AllSectors = new List<ReferenceDataItem>
                {
                    new ReferenceDataItem
                    {
                        Id = SECTOR,
                        Description = SECTOR,
                        ShortDescription = SECTOR
                    }
                },
                PledgeSectors = new List<string>
                {
                    "NoMatch"
                },
                PledgeJobRoles = new List<string>
                {
                    "NoMatch"
                },
                PledgeLevels = new List<string>(),
                PledgeLocations = new List<string>
                {
                    "NoMatch"
                },
                Location = LOCATION,
                JobRole = JOBROLE,
                Level = 7
            };

            viewModel.SetupModel();

            var matchPercentage = viewModel.MatchPercentage;

            Assert.IsTrue(viewModel.LevelHasMatched);
            Assert.AreEqual("25%", matchPercentage);
        }
    }
}
