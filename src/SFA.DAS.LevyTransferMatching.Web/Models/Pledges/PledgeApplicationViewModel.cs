using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class PledgeApplicationViewModel
    {
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

        public string MatchPercentage()
        {
            var matchPercentage = CalculateWhetherLocationMatch();
            matchPercentage += CalculateWhetherSectorMatch();
            matchPercentage += CalculateTypeOfJobRoleMatch();

            return $"{matchPercentage}%";
        }

        private int CalculateWhetherLocationMatch()
        {
            var locationMatchPercentage = 0;

            if (PledgeLocations.Contains(Location) || !PledgeLocations.Any())
            {
                locationMatchPercentage = MatchingPercentageShare;
            }

            return locationMatchPercentage;
        }

        private int CalculateWhetherSectorMatch()
        {
            var sectorMatchPercentage = 0;

            foreach (var pledgeSector in PledgeSectors)
            {
                if (Sector.Any(sector => pledgeSector == sector))
                {
                    sectorMatchPercentage = MatchingPercentageShare;
                }
            }

            return sectorMatchPercentage;
        }

        private int CalculateTypeOfJobRoleMatch()
        {
            var jobRoleMatchPercentage = 0;

            foreach (var pledgeJobRole in PledgeJobRoles)
            {
                if (pledgeJobRole == TypeOfJobRole)
                {
                    jobRoleMatchPercentage = MatchingPercentageShare;
                }
            }

            return jobRoleMatchPercentage;
        }
    }
}


