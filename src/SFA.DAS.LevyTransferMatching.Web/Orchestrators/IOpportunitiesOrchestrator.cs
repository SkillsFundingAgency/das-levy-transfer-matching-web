using SFA.DAS.LevyTransferMatching.Web.Models.SearchFunding;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IOpportunitiesOrchestrator
    {
        Task<IndexViewModel> GetIndexViewModel();
    }
}
