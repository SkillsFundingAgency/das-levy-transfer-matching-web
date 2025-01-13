using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SFA.DAS.LevyTransferMatching.Infrastructure.Configuration;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions;

public static class ZenDeskExtensions
{
    public static string? GetZenDeskSectionId(this ViewDataDictionary viewData) => GetZendeskConfiguration(viewData)?.SectionId;

    public static string? GetZenDeskSnippetKey(this ViewDataDictionary viewData) => GetZendeskConfiguration(viewData)?.SnippetKey;

    private static ZenDesk? GetZendeskConfiguration(ViewDataDictionary viewData)
        => viewData.TryGetValue(ViewDataKeys.ViewDataKeys.ZenDeskConfiguration, out var section)
            ? section as ZenDesk
            : null;

    public static IHtmlContent SetZenDeskLabels(this IHtmlHelper html, params string[] labels)
    {
        var keywords = string.Join(",", labels
            .Where(label => !string.IsNullOrEmpty(label))
            .Select(label => $"'{EscapeApostrophes(label)}'"));

        // when there are no keywords default to empty string to prevent zen desk matching articles from the url
        var apiCallString = "<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', { labels: ["
                            + (!string.IsNullOrEmpty(keywords) ? keywords : "''")
                            + "] });</script>";

        return new HtmlString(apiCallString);
    }

    private static string EscapeApostrophes(string input) => input.Replace("'", @"\'");
}