using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface IOpportunitiesOrchestrator
    {
        Task<IndexViewModel> GetIndexViewModel();
        Task<DetailViewModel> GetDetailViewModel(string encodedId);
    }
}
