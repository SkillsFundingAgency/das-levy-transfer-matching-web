using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IApplicationsOrchestrator
    {
        Task<GetApplicationsViewModel> GetApplications(GetApplicationsRequest request, CancellationToken cancellationToken = default);
        Task<ApplicationViewModel> GetApplication(ApplicationRequest request);
        Task AcceptFunding(AcceptFundingPostRequest request, CancellationToken cancellationToken = default);
        Task<AcceptedViewModel> GetAcceptedViewModel(AcceptedRequest request);
    }
}