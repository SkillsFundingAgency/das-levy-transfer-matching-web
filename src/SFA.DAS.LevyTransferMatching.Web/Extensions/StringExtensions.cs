using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static bool IsComplete(this string str)
        {
            return !string.IsNullOrEmpty(str) && str != NotEntered;
        }

        public static bool IsComplete(this IEnumerable<string> list)
        {
            return list != null && list.Any();
        }
    }
}