﻿using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.LevyTransferMatching.Web.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HideAccountNavigationAttribute : ResultFilterAttribute
{
    private bool HideNavigation { get; }
    private bool HideNavigationLinks { get; }

    public HideAccountNavigationAttribute()
    {
        HideNavigation = false;
        HideNavigationLinks = false;
    }

    public HideAccountNavigationAttribute(bool hideNavigation, bool hideNavigationLinks = false)
    {
        HideNavigation = hideNavigation;
        HideNavigationLinks = hideNavigationLinks;
    }

    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Controller is not Controller controller)
        {
            return;
        }
            
        controller.ViewData[ViewDataKeys.ViewDataKeys.HideAccountNavigation] = HideNavigation;
        controller.ViewData[ViewDataKeys.ViewDataKeys.ShowNav] = !HideNavigationLinks;
    }
}