using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class PledgeApplicationViewModel
    {
        private int _matchPercentage;
        public string Location { get; set; }
        public IEnumerable<string> Sector { get; set; }
        public string TypeOfJobRole { get; set; }
        public int Level { get; set; }
        public int NumberOfApprentices { get; set; }
        public int EstimatedDurationMonths { get; set; }
        public DateTime StartBy { get; set; }
        public bool HasTrainingProvider { get; set; }
        public string AboutOpportunity { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<string> EmailAddresses { get; set; }
        public string BusinessWebsite { get; set; }
        public string EmployerAccountName { get; set; }
        public IEnumerable<string> PledgeSectors { get; set; }
        public IEnumerable<string> PledgeLevels { get; set; }
        public IEnumerable<string> PledgeJobRoles { get; set; }
        public IEnumerable<string> PledgeLocations { get; set; }

        private const int MatchingPercentageShare = 25;

        public bool LocationHasMatched { get; private set; }
        public bool SectorHasMatched { get; private set; }
        public bool JobRoleHasMatched { get; private set; }
        public bool LevelHasMatched { get; private set; }

        public string MatchPercentageCssClass
        {
            get
            {
                return _matchPercentage switch
                {
                    100 => "green",
                    75 => "yellow",
                    _ => "red"
                };
            }
        }

        public string MatchPercentage()
        {
            CalculateMatchPercentage();

            return $"{_matchPercentage}%";
        }

        private void CalculateMatchPercentage()
        {
            _matchPercentage = 0;
            _matchPercentage = CalculateWhetherLocationMatch();
            _matchPercentage += CalculateWhetherSectorMatch();
            _matchPercentage += CalculateWhetherTypeOfJobRoleMatch();
            _matchPercentage += CalculateWhetherLevelMatch();
        }

        private int CalculateWhetherLocationMatch()
        {
            var locationMatchPercentage = 0;

            if (!PledgeLocations.Contains(Location) && PledgeLocations.Any())
            {
                return locationMatchPercentage;
            }

            locationMatchPercentage = MatchingPercentageShare;
            LocationHasMatched = true;

            return locationMatchPercentage;
        }

        private int CalculateWhetherSectorMatch()
        {
            var sectorMatchPercentage = 0;

            if (!PledgeSectors.Any())
            {
                sectorMatchPercentage = SetSectorMatchPercentageProperties();
            }
            else
            {
                foreach (var pledgeSector in PledgeSectors)
                {
                    if (Sector.All(sector => pledgeSector != sector))
                    {
                        continue;
                    }

                    sectorMatchPercentage = SetSectorMatchPercentageProperties();
                }
            }
            
            return sectorMatchPercentage;
        }

        private int CalculateWhetherTypeOfJobRoleMatch()
        {
            var jobRoleMatchPercentage = 0;

            if (!PledgeJobRoles.Any())
            {
                jobRoleMatchPercentage = SetJobRoleMatchPercentageProperties();
            }
            else
            {
                foreach (var pledgeJobRole in PledgeJobRoles)
                {
                    if (string.Compare(pledgeJobRole, TypeOfJobRole, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        continue;
                    }

                    jobRoleMatchPercentage = SetJobRoleMatchPercentageProperties();
                }
            }

            return jobRoleMatchPercentage;
        }

        private int CalculateWhetherLevelMatch()
        {
            var levelMatchPercentage = 0;

            if (!PledgeLevels.Any())
            {
                levelMatchPercentage = SetLevelMatchPercentageProperties();
            }
            else
            {
                foreach (var pledgeLevel in PledgeLevels)
                {
                    int.TryParse(pledgeLevel.Substring(pledgeLevel.Length - 1), out var pledgeLevelIntValue);

                    if (pledgeLevelIntValue != Level)
                    {
                        continue;
                    }

                    levelMatchPercentage = SetLevelMatchPercentageProperties();
                }
            }

            return levelMatchPercentage;
        }

        private int SetJobRoleMatchPercentageProperties()
        {
            var jobRoleMatchPercentage = MatchingPercentageShare;
            JobRoleHasMatched = true;

            return jobRoleMatchPercentage;
        }

        private int SetLevelMatchPercentageProperties()
        {
            var levelMatchPercentage = MatchingPercentageShare;
            LevelHasMatched = true;

            return levelMatchPercentage;
        }

        private int SetSectorMatchPercentageProperties()
        {
            var sectorMatchPercentage = MatchingPercentageShare;
            SectorHasMatched = true;

            return sectorMatchPercentage;
        }
    }
}


