using SFA.DAS.LevyTransferMatching.Web.Models.Location;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators;

public interface ILocationOrchestrator
{
    Task<LocationsTypeAheadViewModel> GetLocationsTypeAheadViewModel(string searchTerm);
}