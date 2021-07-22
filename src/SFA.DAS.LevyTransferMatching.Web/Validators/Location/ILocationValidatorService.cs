using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Location
{
    public interface ILocationValidatorService
    {
        Task<Dictionary<int, string>> ValidateLocations(LocationPostRequest request);
    }
}
