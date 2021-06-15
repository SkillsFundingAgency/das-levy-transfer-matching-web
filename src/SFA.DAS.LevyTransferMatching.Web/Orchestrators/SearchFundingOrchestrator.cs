using SFA.DAS.LevyTransferMatching.Infrastructure.Services.SearchFundingService;
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

        public SearchFundingOrchestrator(ISearchFundingService searchFundingService)
        {
            _searchFundingService = searchFundingService;
        }

        public async Task<SearchFundingViewModel> GetSearchFundingViewModel()
        {
            var opportunitiesDto = await _searchFundingService.GetAllOpportunities();
            List<Opportunity> opportunities = opportunitiesDto.Select(x => new Opportunity { EmployerName = x.EmployerName, ReferenceNumber = x.ReferenceNumber }).ToList();

            return new SearchFundingViewModel { Opportunities = opportunities };
        }
    }
}
