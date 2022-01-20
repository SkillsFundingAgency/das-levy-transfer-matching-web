using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Services.SortingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.Services
{
    [TestFixture]
    public class SortingServiceTests
    {
        private ISortingService sortingService;
        private List<ApplicationViewModel> applicationViewModels;

        [SetUp]
        public void Setup()
        {
            sortingService = new SortingService();
            applicationViewModels = new List<ApplicationViewModel>()
            {
                new ApplicationViewModel
                {
                    DasAccountName = "Middle Co.",
                    Amount = 10000,
                    Duration = 12,
                    CreatedOn = new DateTime(2022, 5, 1)
                },
                new ApplicationViewModel
                {
                    DasAccountName = "Acme Ltd",
                    Amount = 2000,
                    Duration = 24,
                    IsJobRoleMatch = true,
                    CreatedOn = new DateTime(2022, 1, 1)
                },
                new ApplicationViewModel
                {
                    DasAccountName = "Zebra Time",
                    Amount = 50000,
                    Duration = 12,
                    IsJobRoleMatch = true,
                    IsSectorMatch = true,
                    IsLevelMatch = true,
                    CreatedOn = new DateTime(2020, 5, 1)
                }
            };
        }

        [TestCase(SortOrder.Descending)]
        [TestCase(SortOrder.Ascending)]
        public void SortByApplicantSortsCorrectly(SortOrder sortOrder)
        {
            List<ApplicationViewModel> expected;

            if (sortOrder == SortOrder.Ascending)
                expected = applicationViewModels.OrderBy(x => x.DasAccountName).ToList();
            else
                expected = applicationViewModels.OrderByDescending(x => x.DasAccountName).ToList();

            var result = sortingService.SortApplications(applicationViewModels, SortColumn.Applicant, sortOrder);

            CollectionAssert.AreEqual(expected, result);
        }

        [TestCase(SortOrder.Descending)]
        [TestCase(SortOrder.Ascending)]
        public void SortByEstimatedTotalCostSortsCorrectly(SortOrder sortOrder)
        {
            List<ApplicationViewModel> expected;

            if (sortOrder == SortOrder.Ascending)
                expected = applicationViewModels.OrderBy(x => x.Amount).ThenBy(x => x.DasAccountName).ToList();
            else
                expected = applicationViewModels.OrderByDescending(x => x.Amount).ThenBy(x => x.DasAccountName).ToList();

            var result = sortingService.SortApplications(applicationViewModels, SortColumn.EstimatedTotalCost, sortOrder);

            CollectionAssert.AreEqual(expected, result);
        }

        [TestCase(SortOrder.Descending)]
        [TestCase(SortOrder.Ascending)]
        public void SortByTypicalDurationSortsCorrectly(SortOrder sortOrder)
        {
            List<ApplicationViewModel> expected;

            if (sortOrder == SortOrder.Ascending)
                expected = applicationViewModels.OrderBy(x => x.Duration).ThenBy(x => x.DasAccountName).ToList();
            else
                expected = applicationViewModels.OrderByDescending(x => x.Duration).ThenBy(x => x.DasAccountName).ToList();

            var result = sortingService.SortApplications(applicationViewModels, SortColumn.TypicalDuration, sortOrder);

            CollectionAssert.AreEqual(expected, result);
        }

        [TestCase(SortOrder.Descending)]
        [TestCase(SortOrder.Ascending)]
        public void SortByCriteriaSortsCorrectly(SortOrder sortOrder)
        {
            List<ApplicationViewModel> expected;

            if (sortOrder == SortOrder.Ascending)
                expected = applicationViewModels.OrderBy(x => x.GetCriteriaScore()).ThenBy(x => x.DasAccountName).ToList();
            else
                expected = applicationViewModels.OrderByDescending(x => x.GetCriteriaScore()).ThenBy(x => x.DasAccountName).ToList();

            var result = sortingService.SortApplications(applicationViewModels, SortColumn.Criteria, sortOrder);

            CollectionAssert.AreEqual(expected, result);
        }

        [TestCase(SortOrder.Descending)]
        [TestCase(SortOrder.Ascending)]
        public void SortByStatusSortsCorrectly(SortOrder sortOrder)
        {
            List<ApplicationViewModel> expected;

            if (sortOrder == SortOrder.Ascending)
                expected = applicationViewModels.OrderBy(x => x.Status).ThenBy(x => x.DasAccountName).ToList();
            else
                expected = applicationViewModels.OrderByDescending(x => x.Status).ThenBy(x => x.DasAccountName).ToList();

            var result = sortingService.SortApplications(applicationViewModels, SortColumn.Status, sortOrder);

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void SortByDefaultSortsByDateCreated()
        {
            List<ApplicationViewModel> expected = applicationViewModels.OrderByDescending(x => x.CreatedOn).ThenBy(x => x.DasAccountName).ToList();

            var result = sortingService.SortApplications(applicationViewModels, null, SortOrder.Descending);

            CollectionAssert.AreEqual(expected, result);
        }
    }
}
