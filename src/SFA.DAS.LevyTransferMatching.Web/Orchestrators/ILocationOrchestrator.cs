using SFA.DAS.LevyTransferMatching.Web.Models.Location;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators
{
    public interface ILocationOrchestrator
    {
        Task<LocationsTypeAheadViewModel> GetLocationsTypeAheadViewModel(string searchTerm);
    }
}
