using SFA.DAS.LevyTransferMatching.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.LocationService
{
    public interface ILocationService
    {
        Task<LocationDto> SearchLocation(string searchTerm);
    }
}
