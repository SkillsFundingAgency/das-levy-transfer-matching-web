using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class StringExtensions
    {
        private const string All = "All";
        private const string NotEntered = "-";

        public static string ToReferenceDataDescriptionList(this IEnumerable<string> selectedReferenceDataItemIds, IEnumerable<ReferenceDataItem> allReferenceDataItems, Func<ReferenceDataItem, string> descriptionSource = null)
        {
            string descriptions = null;

            if (allReferenceDataItems.Count() == selectedReferenceDataItemIds.Count() || selectedReferenceDataItemIds.Count() <= 0)
            {
                descriptions = All;
            }
            else
            {
                var selectedReferenceDataDescriptions = allReferenceDataItems
                    .Where(x => selectedReferenceDataItemIds.Contains(x.Id))
                    .Select(x => descriptionSource == null ? x.Description : descriptionSource(x));

                descriptions = string.Join(", ", selectedReferenceDataDescriptions);
            }

            return descriptions;
        }

        public static string ToLocationsList(this IEnumerable<string> locations)
        {
            if (locations.Count() == 0)
                return All;

            var shortLocationNamesList = locations.Select(x => x.Split(',')[0]);
            return string.Join(", ", shortLocationNamesList);
        }

        public static bool IsComplete(this string str)
        {
            return !string.IsNullOrEmpty(str) && str != NotEntered;
        }

        public static bool IsComplete(this IEnumerable<string> list)
        {
            return list != null && list.Any();
        }

        public static int CalculateWhetherLocationMatch(this IEnumerable<string> pledgeLocations, string location)
        {
            var locationMatchPercentage = 0;

            if (!pledgeLocations.Contains(location) && pledgeLocations.Any())
            {
                return locationMatchPercentage;
            }

            locationMatchPercentage = ApplicationViewModel.MatchingPercentageShare;

            return locationMatchPercentage;
        }

        public static int CalculateWhetherSectorMatch(this IEnumerable<string> pledgeSectors, IEnumerable<string> sector)
        {
            var sectorMatchPercentage = 0;

            if (!pledgeSectors.Any())
            {
                sectorMatchPercentage = ApplicationViewModel.MatchingPercentageShare;
            }
            else
            {
                if (pledgeSectors.Any(pledgeSector => !sector.All(sector => pledgeSector != sector)))
                {
                    sectorMatchPercentage = ApplicationViewModel.MatchingPercentageShare;
                }
            }

            return sectorMatchPercentage;
        }

        public static int CalculateWhetherJobRoleMatch(this IEnumerable<string> pledgeJobRoles, string jobRole)
        {
            var jobRoleMatchPercentage = 0;

            if (!pledgeJobRoles.Any())
            {
                jobRoleMatchPercentage = ApplicationViewModel.MatchingPercentageShare;
            }
            else
            {
                if (pledgeJobRoles.Any(pledgeJobRole => string.Compare(pledgeJobRole, jobRole, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    jobRoleMatchPercentage = ApplicationViewModel.MatchingPercentageShare;
                }
            }

            return jobRoleMatchPercentage;
        }

        public static int CalculateWhetherLevelMatch(this IEnumerable<string> pledgeLevels, int level)
        {
            var levelMatchPercentage = 0;

            if (!pledgeLevels.Any())
            {
                levelMatchPercentage = ApplicationViewModel.MatchingPercentageShare;
            }
            else
            {
                foreach (var pledgeLevel in pledgeLevels)
                {
                    int.TryParse(pledgeLevel.Substring(pledgeLevel.Length - 1), out var pledgeLevelIntValue);

                    if (pledgeLevelIntValue != level)
                    {
                        continue;
                    }

                    levelMatchPercentage = ApplicationViewModel.MatchingPercentageShare;
                }
            }

            return levelMatchPercentage;
        }
    }
}