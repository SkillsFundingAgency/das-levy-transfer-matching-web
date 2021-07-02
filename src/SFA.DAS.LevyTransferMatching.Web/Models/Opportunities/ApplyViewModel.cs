﻿using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ApplyViewModel : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }
        public string JobRole { get; set; }
        public string NumberOfApprentices { get; set; }
        public string StartBy { get; set; }
        public string HaveTrainingProvider { get; set; }
        public string Sectors { get; set; }
        public string Locations { get; set; }

        public string MoreDetail { get; set; }
        public string ContactName { get; set; }
        public string EmailAddress { get; set; }
        public string WebsiteUrl { get; set; }

        public OpportunitySummaryViewModel OpportunitySummaryViewModel { get; set; }

        public bool IsApprenticeshipTrainingSectionComplete { get; set; }
        public bool IsBusinessDetailsSectionComplete { get; set; }
        public bool IsContactDetailsSectionComplete { get; set; }
        public bool IsComplete => IsApprenticeshipTrainingSectionComplete
                                  && IsBusinessDetailsSectionComplete
                                  && IsContactDetailsSectionComplete;        
    }   
}