using SFA.DAS.LevyTransferMatching.Infrastructure.Services.SearchFundingService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService;
using SFA.DAS.LevyTransferMatching.Web.Models.SearchFunding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public class SearchFundingOrchestrator : ISearchFundingOrchestrator
    {
        private readonly ISearchFundingService _searchFundingService;
        private readonly ITagService _tagService;

        public SearchFundingOrchestrator(ISearchFundingService searchFundingService, ITagService tagService)
        {
            _searchFundingService = searchFundingService;
            _tagService = tagService;
        }

        public async Task<SearchFundingViewModel> GetSearchFundingViewModel()
        {
            var opportunitiesDto = _searchFundingService.GetAllOpportunities();
            var levelsTask = _tagService.GetLevels();
            var sectorsTask = _tagService.GetSectors();
            var jobRolesTask = _tagService.GetJobRoles();

            await Task.WhenAll(opportunitiesDto, levelsTask, sectorsTask, jobRolesTask);

            foreach(var opportunity in opportunitiesDto.Result)
            {
                opportunity.Levels = levelsTask.Result.Where(x => opportunity.Levels.Contains(x.TagId)).Select(x => x.Description).ToList();
                opportunity.Sectors = sectorsTask.Result.Where(x => opportunity.Sectors.Contains(x.TagId)).Select(x => x.Description).ToList();
                opportunity.JobRoles = jobRolesTask.Result.Where(x => opportunity.JobRoles.Contains(x.TagId)).Select(x => x.Description).ToList();
            }

            List<Opportunity> opportunities = opportunitiesDto.Result
                .Select(x => new Opportunity
                {
                    Amount = x.Amount,
                    EmployerName = x.DasAccountName,
                    ReferenceNumber = x.EncodedPledgeId,
                    Sectors = x.Sectors,
                    JobRoles = x.JobRoles,
                    Levels = x.Levels
                }).ToList();

            return new SearchFundingViewModel 
            { 
                Opportunities = opportunities
            };
        }
    }
}
