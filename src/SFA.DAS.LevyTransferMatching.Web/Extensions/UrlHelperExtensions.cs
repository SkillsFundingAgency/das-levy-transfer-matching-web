using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.LevyTransferMatching.Web.Helpers;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        private const string AccountsController = "accounts";

        public static string LevyMatchingTransfersAction(this IUrlHelper helper, string path)
        {
            var request = helper.ActionContext.HttpContext.Request;

            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            return AccountAction(helper, baseUrl, path);
        }

        private static string AccountAction(IUrlHelper helper, string baseUrl, string path)
        {
            return Action(baseUrl, PathWithHashedAccountId(helper, AccountsController, path));
        }

        private static string Action(string baseUrl, string path)
        {
            var trimmedBaseUrl = baseUrl.TrimEnd('/');

            return $"{trimmedBaseUrl}/{path}".TrimEnd('/');
        }

        private static string PathWithHashedAccountId(IUrlHelper helper, string controller, string path)
        {
            var hashedAccountId = helper.ActionContext.RouteData.Values[ControllerConstants.AccountHashedIdRouteKeyName];
            return hashedAccountId == null ? $"{controller}/{path}" : $"{controller}/{hashedAccountId}/{path}";
        }
    }
}