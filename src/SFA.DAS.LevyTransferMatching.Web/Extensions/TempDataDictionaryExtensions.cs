using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions;

public static class TempDataDictionaryExtensions
{
    public const string FlashMessageTitleTempDataKey = "FlashMessageTitle";
    public const string FlashMessageBodyTempDataKey = "FlashMessageBody";
    public const string FlashMessageTempDetailKey = "FlashMessageDetail";
    public const string FlashMessageLevelTempDataKey = "FlashMessageLevel";

    public enum FlashMessageLevel
    {
        Info,
        Warning,
        Success
    }


    public static void AddFlashMessage(this ITempDataDictionary tempData, string title, string body, FlashMessageLevel level)
    {
        tempData[FlashMessageBodyTempDataKey] = body;
        tempData[FlashMessageTitleTempDataKey] = title;
        tempData[FlashMessageLevelTempDataKey] = level;
    }
}