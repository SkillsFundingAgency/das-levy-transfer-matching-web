﻿using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class DetailViewModel : OpportunitiesPostRequest
    {
        public OpportunityDto Opportunity { get; set; }
        public string SectorList { get; set; }
        public string JobRoleList { get; set; }
        public string LevelList { get; set; }
        public string YearDescription { get; set; }
    }
}