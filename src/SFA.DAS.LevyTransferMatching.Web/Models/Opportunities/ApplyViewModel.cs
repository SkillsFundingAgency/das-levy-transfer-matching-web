using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;
using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Opportunities
{
    public class ApplyViewModel : ApplyRequest
    {
        public string EncodedPledgeId { get; set; }
        public string JobRole { get; set; }
        public string NumberOfApprentices { get; set; }
        public string StartBy { get; set; }
        public string HaveTrainingProvider { get; set; }
        public List<ReferenceDataItem> SectorOptions { get; set; }
        public List<string> Sectors { get; set; }
        public string Location { get; set; }

        public string MoreDetail { get; set; }
        public string ContactName { get; set; }
        public IEnumerable<string> EmailAddresses { get; set; }
        public string WebsiteUrl { get; set; }

        public OpportunitySummaryViewModel OpportunitySummaryViewModel { get; set; }

        public bool IsApprenticeshipTrainingSectionComplete => JobRole.IsComplete()
                                                               && NumberOfApprentices.IsComplete()
                                                               && StartBy.IsComplete()
                                                               && HaveTrainingProvider.IsComplete();

        public bool IsBusinessDetailsSectionComplete => Sectors.IsComplete()
                                                        && Locations.IsComplete();

        public bool IsContactDetailsSectionComplete => ContactName.IsComplete()
                                                       && EmailAddress.IsComplete()
                                                       && WebsiteUrl.IsComplete();
        
        public bool IsComplete => IsApprenticeshipTrainingSectionComplete
                                  && IsBusinessDetailsSectionComplete
                                  && IsContactDetailsSectionComplete;
    }   
}