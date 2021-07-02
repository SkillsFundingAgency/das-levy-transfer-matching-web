using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class StringExtensions
    {
        private const string All = "All";

        public static string ToReferenceDataDescriptionList(this IEnumerable<string> selectedReferenceDataItemIds, IEnumerable<ReferenceDataItem> allReferenceDataItems)
        {
            string descriptions = null;

            if (allReferenceDataItems.Count() == selectedReferenceDataItemIds.Count())
            {
                descriptions = All;
            }
            else
            {
                var selectedReferenceDataDescriptions = allReferenceDataItems
                    .Where(x => selectedReferenceDataItemIds.Contains(x.Id))
                    .Select(x => x.Description);

                descriptions = string.Join(", ", selectedReferenceDataDescriptions);
            }

            return descriptions;
        }
    }
}