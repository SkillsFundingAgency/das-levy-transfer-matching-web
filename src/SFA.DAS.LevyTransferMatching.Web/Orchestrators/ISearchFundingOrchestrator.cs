using SFA.DAS.LevyTransferMatching.Web.Models.SearchFunding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface ISearchFundingOrchestrator
    {
        Task<SearchFundingViewModel> GetSearchFundingViewModel();
    }
}
