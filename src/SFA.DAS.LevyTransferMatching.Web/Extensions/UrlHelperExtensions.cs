using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using SFA.DAS.LevyTransferMatching.Web.Helpers;
using System;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        private const string AccountsController = "accounts";

        public static string EmployerFinanceAction(this IUrlHelper helper, string path)
        {
            EmployerFinanceWeb employerFinanceWeb = (EmployerFinanceWeb)helper.ActionContext.HttpContext.RequestServices.GetService(typeof(EmployerFinanceWeb));

            var baseUrl = employerFinanceWeb.BaseUrl;

            return AccountAction(helper, baseUrl, path);
        }

        public static string LevyMatchingTransfersAction(this IUrlHelper helper, string path)
        {
            LevyTransferMatchingWeb levyTransferMatchingWeb = (LevyTransferMatchingWeb)helper.ActionContext.HttpContext.RequestServices.GetService(typeof(LevyTransferMatchingWeb));

            var baseUrl = levyTransferMatchingWeb.BaseUrl;

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