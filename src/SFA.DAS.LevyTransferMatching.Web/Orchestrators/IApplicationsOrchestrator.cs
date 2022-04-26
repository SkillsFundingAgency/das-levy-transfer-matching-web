using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IApplicationsOrchestrator
    {
        Task<GetApplicationsViewModel> GetApplications(GetApplicationsRequest request, CancellationToken cancellationToken = default);
        Task<ApplicationViewModel> GetApplication(ApplicationRequest request);
        Task SetApplicationAcceptance(ApplicationPostRequest request);
        Task<AcceptedViewModel> GetAcceptedViewModel(AcceptedRequest request);
        Task<DeclinedViewModel> GetDeclinedViewModel(DeclinedRequest request);
        Task<WithdrawnViewModel> GetWithdrawnViewModel(WithdrawnRequest request);
        Task<WithdrawalConfirmationViewModel> GetWithdrawalConfirmationViewModel(WithdrawalConfirmationRequest request);
        Task WithdrawApplicationAfterAcceptance(ConfirmWithdrawalPostRequest request);
    }
}