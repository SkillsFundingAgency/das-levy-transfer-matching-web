using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System.Collections.Generic;

namespace SFA.DAS.LevyTransferMatching.Web.Services.SortingService
{
    public interface ISortingService
    {
        List<ApplicationsViewModel.Application> SortApplications(List<ApplicationsViewModel.Application> applications, SortColumn? sortColumn, SortOrder? sortOrder);
    }
}
