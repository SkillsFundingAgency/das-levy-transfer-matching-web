using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class OpportunitiesOrchestrator : IOpportunitiesOrchestrator
    {
        private readonly IOpportunitiesService _opportunitiesService;

        public OpportunitiesOrchestrator(IOpportunitiesService opportunitiesService)
        {
            _opportunitiesService = opportunitiesService;
        }

        public async Task<IndexViewModel> GetIndexViewModel()
        {
            var opportunitiesDto = await _opportunitiesService.GetAllOpportunities();
            List<Opportunity> opportunities = opportunitiesDto.Select(x => new Opportunity { EmployerName = x.DasAccountName, ReferenceNumber = x.EncodedPledgeId }).ToList();

            return new IndexViewModel { Opportunities = opportunities };
        }
    }
}
