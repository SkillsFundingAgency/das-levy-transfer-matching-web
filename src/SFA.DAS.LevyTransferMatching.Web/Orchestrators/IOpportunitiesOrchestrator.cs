﻿using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IOpportunitiesOrchestrator
    {
        Task<IndexViewModel> GetIndexViewModel();
        Task<DetailViewModel> GetDetailViewModel(int pledgeId);
        Task<ApplyViewModel> GetApplyViewModel(ApplicationRequest request);
        Task<MoreDetailsViewModel> GetMoreDetailsViewModel(MoreDetailsRequest request);
        Task UpdateCacheItem(MoreDetailsPostRequest request);
        Task UpdateCacheItem(ApplicationDetailsPostRequest request);
        Task UpdateCacheItem(SectorPostRequest request);
        Task<string> GetUserEncodedAccountId();
        Task<ApplicationDetailsViewModel> GetApplicationViewModel(ApplicationDetailsRequest request);
        Task<ApplicationRequest> PostApplicationViewModel(ApplicationDetailsPostRequest request);
        Task<SectorViewModel> GetSectorViewModel(SectorRequest request);
		        Task<ConfirmationViewModel> GetConfirmationViewModel(ConfirmationRequest request);
        Task SubmitApplication(ApplyPostRequest request);
    }
}
