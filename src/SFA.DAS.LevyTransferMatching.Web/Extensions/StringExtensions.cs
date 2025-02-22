﻿using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions;

public static class StringExtensions
{
    private const string All = "All";
    private const string NotEntered = "-";

    public static string ToReferenceDataDescriptionList(this IEnumerable<string> selectedReferenceDataItemIds, IEnumerable<ReferenceDataItem> allReferenceDataItems, Func<ReferenceDataItem, string> descriptionSource = null)
    {
        return ToReferenceDataDescriptionList(selectedReferenceDataItemIds, allReferenceDataItems, ", ", descriptionSource);
    }

    public static string ToReferenceDataDescriptionList(this IEnumerable<string> selectedReferenceDataItemIds, IEnumerable<ReferenceDataItem> allReferenceDataItems, string separator, Func<ReferenceDataItem, string> descriptionSource = null)
    {
        string descriptions;

        // Preventing possible multiple enumerations
        var referenceDataItemsArray = allReferenceDataItems as ReferenceDataItem[] ?? allReferenceDataItems.ToArray();
        var referenceDataItemIdsArray = selectedReferenceDataItemIds as string[] ?? selectedReferenceDataItemIds.ToArray();
        
        if (referenceDataItemsArray.Length == referenceDataItemIdsArray.Length || referenceDataItemIdsArray.Length <= 0)
        {
            descriptions = All;
        }
        else
        {
            var selectedReferenceDataDescriptions = referenceDataItemsArray
                .Where(x => referenceDataItemIdsArray.Contains(x.Id))
                .Select(x => descriptionSource == null ? x.Description : descriptionSource(x));

            descriptions = string.Join(separator, selectedReferenceDataDescriptions);
        }

        return descriptions;
    }

    public static string ToLocationsList(this IEnumerable<string> locations)
    {
        // Preventing possible multiple enumerations
        var locationsArray = locations as string[] ?? locations.ToArray();
        
        if (locationsArray.Length == 0)
            return All;

        var shortLocationNamesList = locationsArray.Select(x => x.Split(',')[0]);
        
        return string.Join(", ", shortLocationNamesList);
    }

    public static bool IsComplete(this string str)
    {
        return !string.IsNullOrEmpty(str) && str != NotEntered;
    }

    public static bool IsComplete(this IEnumerable<string> list)
    {
        return list != null && list.Any();
    }

    public static string ToApplicationLocationsString(this IEnumerable<string> list, string separator, string additionalLocation)
    {
        string applicationLocationsString;

        // Preventing possible multiple enumerations
        var listAsArray = list as string[] ?? list.ToArray();
        
        if (list != null && listAsArray.Length != 0)
        {
            list = listAsArray.Select(x => x.Contains(',') ? x.Split(',')[0] : x);
            applicationLocationsString = string.Join(separator, list);
            if (!string.IsNullOrEmpty(additionalLocation))
            {
                applicationLocationsString = string.Concat(applicationLocationsString, separator, additionalLocation);
            }
        }
        else
        {
            applicationLocationsString = additionalLocation ?? string.Empty;
        }

        return applicationLocationsString;
    }
}