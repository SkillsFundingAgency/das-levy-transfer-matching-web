using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators;

public interface IPledgeOrchestrator
{
    CloseViewModel GetCloseViewModel(CloseRequest request);
    Task<PledgesViewModel> GetPledgesViewModel(PledgesRequest request);
    DetailViewModel GetDetailViewModel(DetailRequest request);
    Task<ApplicationsViewModel> GetApplications(ApplicationsRequest request);
    Task<ApplicationViewModel> GetApplicationViewModel(ApplicationRequest request,
        CancellationToken cancellationToken = default);
    Task<ApplicationApprovedViewModel> GetApplicationApprovedViewModel(ApplicationApprovedRequest request);
    Task SetApplicationOutcome(ApplicationPostRequest request);
    Task<ApplicationApprovalOptionsViewModel> GetApplicationApprovalOptionsViewModel(ApplicationApprovalOptionsRequest request, CancellationToken cancellationToken = default);
    Task SetApplicationApprovalOptions(ApplicationApprovalOptionsPostRequest request, CancellationToken cancellationToken = default);
    Task<byte[]> GetPledgeApplicationsDownloadModel(ApplicationsRequest request);
    Task ClosePledge(ClosePostRequest request);
    Task<RejectApplicationsViewModel> GetRejectApplicationsViewModel(RejectApplicationsRequest request);
    Task RejectApplications(RejectApplicationsPostRequest request);
}