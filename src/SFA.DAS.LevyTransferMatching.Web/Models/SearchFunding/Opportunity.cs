using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Models.SearchFunding
{
    public class Opportunity
    {
        public int Amount { get; set; }
        public string EmployerName { get; set; }
        public string ReferenceNumber { get; set; }
        public List<string> Locations { get; set; }
        public List<string> Sectors { get; set; }
        public List<string> JobRoles { get; set; }
        public List<string> Levels { get; set; }
        public string DisplayLocations => (Locations != null && Locations.Any()) ? string.Join(", ", Locations) : "All";
        public string DisplaySectors => (Sectors != null && Sectors.Any()) ? string.Join(", ", Sectors) : "All";
        public string DisplayJobRoles => (JobRoles != null && JobRoles.Any()) ? string.Join(", ", JobRoles) : "All";
        public string DisplayLevels => (Levels != null && Levels.Any()) ? string.Join(", ", Levels) : "All";
    }
}