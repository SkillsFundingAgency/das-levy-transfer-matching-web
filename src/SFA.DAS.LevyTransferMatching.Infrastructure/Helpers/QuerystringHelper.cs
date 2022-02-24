using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Helpers
{
    public static class QuerystringHelper
    {
        public static string GetFormattedQuerystring(string key, IEnumerable<string> values)
        {
            var queryParams = new NameValueCollection();
            if (values == null || !values.Any()) 
                return string.Empty;
            foreach (var value in values)
                queryParams.Add(key, value);
            return queryParams.ToQueryString();
        }

        public static string ToQueryString(this NameValueCollection nvc)
        {
            if (nvc == null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (string key in nvc.Keys)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;

                string[] values = nvc.GetValues(key);
                if (values == null) continue;

                foreach (string value in values)
                {
                    sb.Append(sb.Length == 0 ? "?" : "&");
                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value));
                }
            }
            return sb.ToString();
        }
    }
}
