﻿using SFA.DAS.LevyTransferMatching.Web.Models.Enums;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class SectorPostRequest : PledgesRequest
    {
        public Sector? Sectors { get; set; }
    }
}