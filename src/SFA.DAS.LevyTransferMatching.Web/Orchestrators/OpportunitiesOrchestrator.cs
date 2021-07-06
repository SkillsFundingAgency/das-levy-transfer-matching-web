using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
using System.Collections.Generic;
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
        private readonly ITagService _tagService;
        private readonly IEncodingService _encodingService;

        public OpportunitiesOrchestrator(IOpportunitiesService opportunitiesService, ITagService tagService, IEncodingService encodingService)
        {
            _opportunitiesService = opportunitiesService;
            _tagService = tagService;
            _encodingService = encodingService;
        }

        public async Task<IndexViewModel> GetIndexViewModel()
        {
            var opportunitiesDto = _opportunitiesService.GetAllOpportunities();
            var levelsTask = _tagService.GetLevels();
            var sectorsTask = _tagService.GetSectors();
            var jobRolesTask = _tagService.GetJobRoles();

            await Task.WhenAll(opportunitiesDto, levelsTask, sectorsTask, jobRolesTask);

            foreach(var opportunity in opportunitiesDto.Result)
            {
                opportunity.Levels = levelsTask.Result.Where(x => opportunity.Levels.Contains(x.Id)).Select(x => x.Description).ToList();
                opportunity.Sectors = sectorsTask.Result.Where(x => opportunity.Sectors.Contains(x.Id)).Select(x => x.Description).ToList();
                opportunity.JobRoles = jobRolesTask.Result.Where(x => opportunity.JobRoles.Contains(x.Id)).Select(x => x.Description).ToList();
            }

            List<Opportunity> opportunities = opportunitiesDto.Result
                .Select(x => new Opportunity
                {
                    Amount = x.Amount,
                    EmployerName = x.DasAccountName,
                    ReferenceNumber = _encodingService.Encode(x.Id, EncodingType.PledgeId),
                    Sectors = x.Sectors,
                    JobRoles = x.JobRoles,
                    Levels = x.Levels
                }).ToList();

            return new IndexViewModel 
            { 
                Opportunities = opportunities
            };
        }
    }
}
