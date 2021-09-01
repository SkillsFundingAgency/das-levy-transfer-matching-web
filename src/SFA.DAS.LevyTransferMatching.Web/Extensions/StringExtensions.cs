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

        public static int CheckForMatchPercentage(this IEnumerable<string> pledgeListToCompare, string valueToCompare, int percentageMatchIsWorth = 25)
        {
            var matchPercentage = 0;

            if (!pledgeListToCompare.Any())
            {
                matchPercentage = percentageMatchIsWorth;
            }

            if (pledgeListToCompare.Contains(valueToCompare))
            {
                matchPercentage = percentageMatchIsWorth;
            }

            return matchPercentage;
        }
        
        public static int CheckForMatchPercentage(this IEnumerable<string> pledgeListToCompare, int valueToCompare, int percentageMatchIsWorth = 25)
        {
            var levelMatchPercentage = 0;

            if (!pledgeListToCompare.Any())
            {
                levelMatchPercentage = percentageMatchIsWorth;
            }
            else
            {
                foreach (var pledgeLevel in pledgeListToCompare)
                {
                    int.TryParse(pledgeLevel.Substring(pledgeLevel.Length - 1), out var pledgeLevelIntValue);

                    if (pledgeLevelIntValue != valueToCompare)
                    {
                        continue;
                    }

                    levelMatchPercentage = percentageMatchIsWorth;
                    break;
                }
            }

            return levelMatchPercentage;
        }

        public static int CheckForMatchPercentage(this IEnumerable<string> pledgeListToCompare, IEnumerable<string> valueToCompare, int percentageMatchIsWorth = 25)
        {
            var sectorMatchPercentage = 0;

            if (!pledgeListToCompare.Any())
            {
                sectorMatchPercentage = percentageMatchIsWorth;
            }
            else
            {
                if (pledgeListToCompare.Any(pledgeSector => valueToCompare.Any(sector => pledgeSector == sector)))
                {
                    sectorMatchPercentage = percentageMatchIsWorth;
                }
            }

            return sectorMatchPercentage;
        }
    }
}