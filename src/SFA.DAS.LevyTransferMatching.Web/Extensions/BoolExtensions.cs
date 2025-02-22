﻿namespace SFA.DAS.LevyTransferMatching.Web.Extensions;

public static class BoolExtensions
{
    public static string ToApplyViewString(this bool? value)
    {
        return value.HasValue ? value.Value ? "Yes" : "No" : "-";
    }
    
    public static string ToYesNo(this bool value)
    {
        return value ? "Yes" : "No";
    }
}