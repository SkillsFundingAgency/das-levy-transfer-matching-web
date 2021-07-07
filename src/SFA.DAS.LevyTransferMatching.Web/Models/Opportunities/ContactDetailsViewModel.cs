using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ContactDetailsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> EmailAddresses { get; set; }
        public string BusinessWebsite { get; set; }
    }
}