using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EnableGoogleAnalyticsAttribute : ResultFilterAttribute
    {
        private readonly GoogleAnalytics _googleAnalyticsConfiguration;

        public EnableGoogleAnalyticsAttribute(GoogleAnalytics configuration)
        {
            _googleAnalyticsConfiguration = configuration;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            switch (context.Controller)
            {
                case PageModel page:
                    SetViewData(page.ViewData);
                    break;
                case Controller controller:
                    SetViewData(controller.ViewData);
                    break;
            }

            return;

            void SetViewData(ViewDataDictionary viewData)
                => viewData[ViewDataKeys.GoogleAnalyticsConfiguration] = _googleAnalyticsConfiguration;
        }
    }
}
