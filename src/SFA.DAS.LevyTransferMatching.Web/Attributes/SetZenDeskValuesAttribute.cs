using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Web.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SetZenDeskValuesAttribute(ZenDesk zenDeskConfiguration) : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Controller is PageModel page)
        {
            page.ViewData[ViewDataKeys.ViewDataKeys.ZenDeskConfiguration] = zenDeskConfiguration;
        }

        if (context.Controller is Controller controller)
        {
            controller.ViewData[ViewDataKeys.ViewDataKeys.ZenDeskConfiguration] = zenDeskConfiguration;
        }
    }
}