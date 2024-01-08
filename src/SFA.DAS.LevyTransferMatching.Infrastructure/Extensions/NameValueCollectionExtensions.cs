using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static string ToQueryString(this NameValueCollection source)
        {
            if (source.Count == 0) return string.Empty;

            var list = new List<string>();
            foreach (var key in source.AllKeys)
            {
                foreach (var value in source.GetValues(key)!)
                {
                    list.Add($"{WebUtility.UrlEncode(key)}={WebUtility.UrlEncode(value)}");
                }
            }

            return "?" + string.Join("&", list);
        }
    }
}
