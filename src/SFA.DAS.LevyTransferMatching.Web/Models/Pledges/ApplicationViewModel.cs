using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using StructureMap.Query;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class ApplicationViewModel
    {
        public string EncodedApplicationId { get; set; }
        public string DasAccountName { get; set; }
        public int PledgeId { get; set; }
        public string Details { get; set; }
        public string StandardId { get; set; }
        public int NumberOfApprentices { get; set; }
        public DateTime StartDate { get; set; }
        public int Amount { get; set; }
        public string DisplayAmount => Amount.ToString("C0", new CultureInfo("en-GB"));
        public bool HasTrainingProvider { get; set; }
        public IEnumerable<string> Sectors { get; set; }
        public string Postcode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessWebsite { get; set; }
        public IEnumerable<string> EmailAddresses { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; } //TODO: For TM-47 this is always Awaiting approval. Will be completed with a later story
        public StandardsListItemDto Standard { get; set; }
        private int _locationMatchPercentage;
        private int _sectorMatchedPercentage;
        private int _jobRoleMatchPercentage;
        private int _levelsMatchedPercentage;
        public string Location { get; set; }
        public string JobRole { get; set; }
        public int Level { get; set; }
        public int EstimatedDurationMonths { get; set; }
        public DateTime StartBy { get; set; }
        public string AboutOpportunity { get; set; }
        public string EmployerAccountName { get; set; }
        public IEnumerable<string> PledgeSectors { get; set; }
        public IEnumerable<string> PledgeLevels { get; set; }
        public IEnumerable<string> PledgeJobRoles { get; set; }
        public IEnumerable<string> PledgeLocations { get; set; }
        public const int MatchingPercentageShare = 25;
        public bool LocationHasMatched => _locationMatchPercentage > 0;
        public bool SectorHasMatched => _sectorMatchedPercentage > 0;
        public bool JobRoleHasMatched => _jobRoleMatchPercentage > 0;
        public bool LevelHasMatched => _levelsMatchedPercentage > 0;
        public string MatchPercentageCssClass => (_locationMatchPercentage + _sectorMatchedPercentage + _jobRoleMatchPercentage + _levelsMatchedPercentage).MatchPercentageCssClass();

        public string MatchPercentage
        {
            get
            {
                CalculateMatchPercentage();

                return $"{_locationMatchPercentage + _sectorMatchedPercentage + _jobRoleMatchPercentage + _levelsMatchedPercentage}%";
            }
        }

        public string LocationCssClass => LocationHasMatched.ToTickCssClass();
        public string SectorCssClass => SectorHasMatched.ToTickCssClass();
        public string LevelCssClass => LevelHasMatched.ToTickCssClass();
        public string JobRoleCssClass => JobRoleHasMatched.ToTickCssClass();

        private void CalculateMatchPercentage()
        {
            _locationMatchPercentage = PledgeLocations.CalculateWhetherLocationMatch(Location);
            _sectorMatchedPercentage = PledgeSectors.CalculateWhetherSectorMatch(Sectors);
            _jobRoleMatchPercentage = PledgeJobRoles.CalculateWhetherJobRoleMatch(JobRole);
            _levelsMatchedPercentage = PledgeLevels.CalculateWhetherLevelMatch(Level);
        }
    }
}
