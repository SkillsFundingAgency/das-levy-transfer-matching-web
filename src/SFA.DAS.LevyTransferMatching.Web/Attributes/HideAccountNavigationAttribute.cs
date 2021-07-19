using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.LevyTransferMatching.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HideAccountNavigationAttribute : ResultFilterAttribute
    {
        private bool HideNavigation { get; }

        public HideAccountNavigationAttribute()
        {
            HideNavigation = false;
        }

        public HideAccountNavigationAttribute(bool hideNavigation)
        {
            HideNavigation = hideNavigation;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!(context.Controller is Controller controller)) return;
            controller.ViewData[ViewDataKeys.HideAccountNavigation] = HideNavigation;
        }
    }
}