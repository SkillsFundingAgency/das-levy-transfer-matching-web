using SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Validators
{
    public interface IValidatorService
    {
        Task<Dictionary<int, string>> ValidateLocations(LocationPostRequest request);
    }
}
