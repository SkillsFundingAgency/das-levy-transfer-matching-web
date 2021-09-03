using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using StructureMap.Query;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class GetApplicationViewModel
    {
        public GetApplicationViewModel()
        {
            
        }

        public void SetupModel()
        {
            _displaySectors = Sectors.ToReferenceDataDescriptionList(AllSectors);
           
        }

        private string _displaySectors;

        public string DisplaySectors => _displaySectors;
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
        public string Location { get; set; }
        public string JobRole { get; set; }
        public int Level { get; set; }
        public int EstimatedDurationMonths { get; set; }
        public DateTime StartBy { get; set; }
        public string AboutOpportunity { get; set; }
        public string EmployerAccountName { get; set; }
        public IEnumerable<ReferenceDataItem> AllSectors { get; set; }
        public List<string> PledgeSectors { get; set; }
        public List<string> PledgeJobRoles { get; set; }
        public List<string> PledgeLevels { get; set; }
        public List<string> PledgeLocations { get; set; }
    }
}
