using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class OpportunitiesOrchestrator : IOpportunitiesOrchestrator
    {
        private readonly IOpportunitiesService _opportunitiesService;
        private readonly IEncodingService _encodingService;

        public OpportunitiesOrchestrator(IOpportunitiesService opportunitiesService, IEncodingService encodingService)
        {
            _opportunitiesService = opportunitiesService;
            _encodingService = encodingService;
        }

        public async Task<IndexViewModel> GetIndexViewModel()
        {
            var opportunitiesDto = await _opportunitiesService.GetAllOpportunities();
            var opportunities = opportunitiesDto.Select(x => new Opportunity
                {
                    EmployerName = x.DasAccountName,
                    ReferenceNumber = _encodingService.Encode(x.Id, EncodingType.PledgeId)
                })
                .ToList();

            return new IndexViewModel { Opportunities = opportunities };
        }
    }
}
