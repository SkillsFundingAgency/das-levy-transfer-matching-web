using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Helpers
{
    public class QuerystringHelper
    {
        public static string GetFormattedQuerystring(string prefix, string key, IEnumerable<string> values)
        {
            var result = string.Empty;
            if (values != null && values.Any())
            {
                foreach (var value in values)
                {
                    result = result + (value == values.First() ? "" : "&") + key + "=" + value;
                }
                result = prefix + result;
            }
            return result;
        }
    }
}
