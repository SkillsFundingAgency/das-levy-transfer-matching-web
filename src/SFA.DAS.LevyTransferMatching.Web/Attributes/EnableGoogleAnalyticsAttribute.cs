using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;
using System;

namespace SFA.DAS.LevyTransferMatching.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EnableGoogleAnalyticsAttribute : ResultFilterAttribute
    {
        public GoogleAnalytics GoogleAnalyticsConfiguration { get; }

        public EnableGoogleAnalyticsAttribute(GoogleAnalytics configuration)
        {
            GoogleAnalyticsConfiguration = configuration;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Controller is PageModel page)
                SetViewData(page.ViewData);

            if (context.Controller is Controller controller)
                SetViewData(controller.ViewData);

            void SetViewData(ViewDataDictionary viewData)
                => viewData[ViewDataKeys.GoogleAnalyticsConfiguration] = GoogleAnalyticsConfiguration;
        }
    }
}
