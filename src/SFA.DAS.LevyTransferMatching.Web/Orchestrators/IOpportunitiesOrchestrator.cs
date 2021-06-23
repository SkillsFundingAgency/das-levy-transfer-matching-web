﻿using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IOpportunitiesOrchestrator
    {
        Task<IndexViewModel> GetIndexViewModel();
        Task<DetailViewModel> GetDetailViewModel(string encodedId);
    }
}