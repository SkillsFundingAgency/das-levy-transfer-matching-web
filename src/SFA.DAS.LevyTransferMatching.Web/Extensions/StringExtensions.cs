using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class StringExtensions
    {
        private const string All = "All";

        public static string ToTagDescriptionList(this IEnumerable<string> selectedTagIds, IEnumerable<Tag> allTags)
        {
            string descriptions = null;

            if (allTags.Count() == selectedTagIds.Count())
            {
                descriptions = All;
            }
            else
            {
                var selectedTagDescriptions = allTags
                    .Where(x => selectedTagIds.Contains(x.TagId))
                    .Select(x => x.Description);

                descriptions = string.Join(", ", selectedTagDescriptions);
            }

            return descriptions;
        }
    }
}