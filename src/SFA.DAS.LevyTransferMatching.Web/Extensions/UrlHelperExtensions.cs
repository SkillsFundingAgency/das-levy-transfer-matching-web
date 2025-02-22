﻿using Microsoft.AspNetCore.Routing;
using SFA.DAS.LevyTransferMatching.Web.Attributes;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions;

public static class UrlHelperExtensions
{
    public static string Action(this IUrlHelper helper, string actionName, object routeValues, bool suppressAutoDecodedProperties)
    {
        var finalRouteValues = new RouteValueDictionary(routeValues);

        if (suppressAutoDecodedProperties)
        {
            RemoveUnboundValues(finalRouteValues, routeValues);
        }

        return helper.Action(actionName, finalRouteValues);
    }

    public static string Action(this IUrlHelper helper, string actionName, string controllerName, object routeValues, bool suppressAutoDecodedProperties)
    {
        var finalRouteValues = new RouteValueDictionary(routeValues);

        if (suppressAutoDecodedProperties)
        {
            RemoveUnboundValues(finalRouteValues, routeValues);
        }

        return helper.Action(actionName, controllerName, finalRouteValues);
    }
    
    private static void RemoveUnboundValues(RouteValueDictionary routeValues, object source)
    {
        if (source == null)
        {
            return;
        }

        var type = source.GetType();

        foreach (var property in type.GetProperties())
        {
            var propertyName = property.Name;

            var hasAttribute = Attribute.IsDefined(property, typeof(AutoDecodeAttribute));

            if (hasAttribute)
            {
                routeValues.Remove(propertyName);
            }
        }
    }
}