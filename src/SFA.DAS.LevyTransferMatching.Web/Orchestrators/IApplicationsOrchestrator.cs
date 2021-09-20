using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IApplicationsOrchestrator
    {
        Task<GetApplicationsViewModel> GetApplications(string hashedAccountId);
    }
}