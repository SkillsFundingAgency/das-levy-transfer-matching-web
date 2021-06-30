﻿using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class LocationPostRequest : PledgesRequest
    {
        public List<string> Locations { get; set; }
        public bool AllSelected { get; set; }

        public Dictionary<int, string> Errors { get; set; }
    }
}
