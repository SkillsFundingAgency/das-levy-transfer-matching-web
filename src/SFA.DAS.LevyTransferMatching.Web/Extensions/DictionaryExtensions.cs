﻿namespace SFA.DAS.LevyTransferMatching.Web.Extensions;

public static class DictionaryExtensions
{
    public static void AddIfNotPresent<TKey, TValue>(this IDictionary<TKey, TValue> headers, TKey key, TValue values)
    {
        if (headers.ContainsKey(key))
        {
            return;
        }

        headers.Add(key, values);
    }
}