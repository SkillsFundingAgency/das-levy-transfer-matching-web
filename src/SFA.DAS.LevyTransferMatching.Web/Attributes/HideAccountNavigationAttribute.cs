using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.LevyTransferMatching.Web.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HideAccountNavigationAttribute(bool hideNavigation, bool hideNavigationLinks = false) : ResultFilterAttribute
{
    private bool HideNavigation { get; } = hideNavigation;
    private bool HideNavigationLinks { get; } = hideNavigationLinks;

    public HideAccountNavigationAttribute() : this(false, false)
    {
    }

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Controller is not Controller controller)
        {
            return;
        }
            
        controller.ViewData[ViewDataKeys.ViewDataKeys.HideAccountNavigation] = HideNavigation;
        controller.ViewData[ViewDataKeys.ViewDataKeys.ShowNav] = !HideNavigationLinks;
        controller.ViewBag.ShowNav = HideNavigation ? false : !HideNavigationLinks;
    }
}