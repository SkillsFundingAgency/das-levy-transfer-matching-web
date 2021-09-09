using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Location
{
    public interface ILocationValidatorService
    {
        Task<Dictionary<int, string>> ValidateLocations(LocationPostRequest request, IDictionary<int, IEnumerable<string>> multipleValidResults);
    }
}