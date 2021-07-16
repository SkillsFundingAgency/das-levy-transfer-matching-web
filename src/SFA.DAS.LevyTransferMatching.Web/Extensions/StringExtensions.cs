using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class StringExtensions
    {
        private const string All = "All";

        [Obsolete("To eventually be replaced with the other method of the same name - please use other overload.")]
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

        public static string ToReferenceDataDescriptionList(this IEnumerable<string> selectedReferenceDataDescriptions, int allReferenceDataItemsCount)
        {
            string descriptions = null;

            if (selectedReferenceDataDescriptions.Count() == allReferenceDataItemsCount || selectedReferenceDataDescriptions.Count() <= 0)
            {
                descriptions = All;
            }
            else
            {
                descriptions = string.Join(", ", selectedReferenceDataDescriptions);
            }

            return descriptions;
        }
    }
}