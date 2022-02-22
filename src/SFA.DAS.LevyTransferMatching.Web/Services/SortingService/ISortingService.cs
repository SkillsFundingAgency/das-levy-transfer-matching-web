using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Services.SortingService
{
    public interface ISortingService
    {
        List<ApplicationViewModel> SortApplications(List<ApplicationViewModel> applications, SortColumn? sortColumn, SortOrder? sortOrder);
    }
}
