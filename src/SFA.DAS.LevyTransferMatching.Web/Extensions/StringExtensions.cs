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

        public static string ToApplicationLocationsString(this IEnumerable<string> list, string separator, string additionalLocation, string specificLocation)
        {
            string applicationLocationsString = "";
            if (list.Any())
            {
                list = list.Select(x => x.Contains(',') ? x.Split(',')[0] : x);
                applicationLocationsString = string.Join(separator, list);
                if (!string.IsNullOrEmpty(additionalLocation))
                {
                    applicationLocationsString = string.Concat(applicationLocationsString, separator, additionalLocation);
                }
            }
            else
            {
                applicationLocationsString = specificLocation;
            }

            return applicationLocationsString;
        }
    }
}