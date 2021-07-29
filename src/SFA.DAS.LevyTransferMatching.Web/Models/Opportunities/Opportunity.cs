using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class Opportunity
    {
        public int Amount { get; set; }
        public string EmployerName { get; set; }
        public string ReferenceNumber { get; set; }
        public IEnumerable<string> Locations { get; set; }
        public string Sectors { get; set; }
        public string JobRoles { get; set; }
        public string Levels { get; set; }
        public string DisplayAmount => Amount.ToString("C0", new CultureInfo("en-GB"));
        public string DisplayLocations => (Locations != null && Locations.Any()) ? string.Join(", ", Locations) : "All";
    }
}