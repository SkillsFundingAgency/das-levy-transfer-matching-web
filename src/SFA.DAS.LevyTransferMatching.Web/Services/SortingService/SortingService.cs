using SFA.DAS.LevyTransferMatching.Domain.Extensions;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Services.SortingService
{
    public class SortingService : ISortingService
    {
        public List<ApplicationsViewModel.Application> SortApplications(List<ApplicationsViewModel.Application> applications, SortColumn? sortColumn, SortOrder? sortOrder)
        {
            switch (sortColumn)
            {
                case SortColumn.Applicant:
                    return SortByApplicant(applications, sortOrder.Value).ToList();
                case SortColumn.EstimatedTotalCost:
                    return SortByEstimatedTotalCost(applications, sortOrder.Value).ThenBy(x => x.DasAccountName).ToList();
                case SortColumn.TypicalDuration:
                    return SortByTypicalDuration(applications, sortOrder.Value).ThenBy(x => x.DasAccountName).ToList();
                case SortColumn.Criteria:
                    return SortByCriteria(applications, sortOrder.Value).ThenBy(x => x.DasAccountName).ToList();
                case SortColumn.Status:
                    return SortByStatus(applications, sortOrder.Value).ThenBy(x => x.DasAccountName).ToList();
                default:
                    return SortByDateApplied(applications).ThenBy(x => x.DasAccountName).ToList();
            }
        }

        public IOrderedEnumerable<ApplicationsViewModel.Application> SortByApplicant(List<ApplicationsViewModel.Application> applications, SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Ascending)
                return applications.OrderBy(x => x.DasAccountName);
            else
                return applications.OrderByDescending(x => x.DasAccountName);
        }

        public IOrderedEnumerable<ApplicationsViewModel.Application> SortByEstimatedTotalCost(List<ApplicationsViewModel.Application> applications, SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Ascending)
                return applications.OrderBy(x => x.Amount);
            else
                return applications.OrderByDescending(x => x.Amount);
        }

        public IOrderedEnumerable<ApplicationsViewModel.Application> SortByTypicalDuration(List<ApplicationsViewModel.Application> applications, SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Ascending)
                return applications.OrderBy(x => x.Duration);
            else
                return applications.OrderByDescending(x => x.Duration);
        }

        public IOrderedEnumerable<ApplicationsViewModel.Application> SortByCriteria(List<ApplicationsViewModel.Application> applications, SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Ascending)
                return applications.OrderBy(x => x.GetCriteriaScore());
            else
                return applications.OrderByDescending(x => x.GetCriteriaScore());
        }

        public IOrderedEnumerable<ApplicationsViewModel.Application> SortByStatus(List<ApplicationsViewModel.Application> applications, SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.Ascending)
                return applications.OrderBy(x => x.Status.GetLabelForSender());
            else
                return applications.OrderByDescending(x => x.Status.GetLabelForSender());
        }

        public IOrderedEnumerable<ApplicationsViewModel.Application> SortByDateApplied(List<ApplicationsViewModel.Application> applications)
        {
            return applications.OrderByDescending(x => x.CreatedOn);
        }
    }
}
