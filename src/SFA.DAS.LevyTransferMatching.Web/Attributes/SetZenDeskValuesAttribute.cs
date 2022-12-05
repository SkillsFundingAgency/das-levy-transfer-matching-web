using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SetZenDeskValuesAttribute : ResultFilterAttribute
    {
        public ZenDesk ZenDeskConfiguration { get; }

        public SetZenDeskValuesAttribute(ZenDesk zenDeskConfiguration)
        {
            ZenDeskConfiguration = zenDeskConfiguration;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Controller is PageModel page)
                page.ViewData[ViewDataKeys.ZenDeskConfiguration] = ZenDeskConfiguration;

            if (context.Controller is Controller controller)
                controller.ViewData[ViewDataKeys.ZenDeskConfiguration] = ZenDeskConfiguration;
        }
    }
}
