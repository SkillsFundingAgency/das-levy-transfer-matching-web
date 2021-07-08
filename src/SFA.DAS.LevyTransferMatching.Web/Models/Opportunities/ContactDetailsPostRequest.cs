﻿namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ContactDetailsPostRequest : ContactDetailsRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] EmailAddresses { get; set; }
        public string BusinessWebsite { get; set; }
    }
}