using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.LevyTransferMatching.Domain.Extensions;
using SFA.DAS.LevyTransferMatching.Domain.Types;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers;

[HtmlTargetElement("sortable-column")]
public class SortableColumnTagHelper : TagHelper
{
    private const string HeaderCssClass = "govuk-table__header";
    private const string LinkCssClass = "govuk-link das-table__sort";

    [HtmlAttributeName("column-name")]
    public SortColumn ColumnName { get; set; }

    [HtmlAttributeName("column-label")]
    public string Label { get; set; }

    [HtmlAttributeName("default")]
    public bool IsDefault { get; set; }

    [HtmlAttributeName("default-order")]
    public SortOrder DefaultSortOrder { get; set; }

    [HtmlAttributeName("column-type")]
    public SortColumnType ColumnType { get; set; } = SortColumnType.Text;

    [HtmlAttributeName("header-class")]
    public string HeaderClass { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    private readonly IUrlHelper _urlHelper;

    public SortableColumnTagHelper(IUrlHelperFactory urlHelperFactory, IActionContextAccessor contextAccessor)
    {
        _urlHelper = urlHelperFactory.GetUrlHelper(contextAccessor.ActionContext);
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var sortColumn = GetSortColumnFromQueryString();
        var sortOrder = GetSortOrderFromQueryString();
        var isSortColumn = sortColumn == ColumnName || (sortColumn == SortColumn.Default && IsDefault);

        output.TagName = "th";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("scope", "col");
        output.Attributes.SetAttribute("class", $"{HeaderCssClass} {HeaderClass}".Trim());

        if (isSortColumn)
        {
            output.Attributes.SetAttribute("aria-sort", sortOrder == SortOrder.Ascending ? "ascending" : "descending");
        }

        output.Content.SetHtmlContent(BuildSortLink(isSortColumn, sortOrder));
    }

    private TagBuilder BuildSortLink(bool isSortColumn, SortOrder sortOrder)
    {
        var linkId = $"sort-{ColumnName}";
        var nextSortOrder = isSortColumn ? sortOrder.Reverse() : DefaultSortOrder;

        var values = new
        {
            SearchTerm = GetSearchTermFromQueryString(),
            SortColumn = ColumnName,
            SortOrder = nextSortOrder.ToString()
        };

        var link = new TagBuilder("a");
        link.Attributes["id"] = linkId;
        link.Attributes["href"] = _urlHelper.Action(
            ViewContext.RouteData.Values["action"] as string,
            ViewContext.RouteData.Values["controller"] as string,
            values, null, null, linkId);
        link.Attributes["data-sort-column"] = ColumnName.ToString();
        link.Attributes["title"] = $"Sort the table by {Label}";
        link.AddCssClass(isSortColumn ? $"{LinkCssClass} {GetSortOrderCssClass(sortOrder)}" : LinkCssClass);
        link.InnerHtml.Append(Label);

        if (isSortColumn)
        {
            var currentSortState = new TagBuilder("span");
            currentSortState.AddCssClass("govuk-visually-hidden");
            currentSortState.InnerHtml.Append($", sorted {GetDirectionText(sortOrder)}");
            link.InnerHtml.AppendHtml(currentSortState);
        }

        return link;
    }

    private static string GetSortOrderCssClass(SortOrder sortOrder)
    {
        return sortOrder == SortOrder.Ascending ? "das-table__sort--asc" : "das-table__sort--desc";
    }

    private string GetDirectionText(SortOrder sortOrder)
    {
        var isAscending = sortOrder == SortOrder.Ascending;

        return ColumnType switch
        {
            SortColumnType.Date => isAscending ? "oldest to newest" : "newest to oldest",
            SortColumnType.Amount => isAscending ? "low to high" : "high to low",
            _ => isAscending ? "A to Z" : "Z to A"
        };
    }

    private SortOrder GetSortOrderFromQueryString()
    {
        if (ViewContext.HttpContext.Request.Query.TryGetValue("SortOrder", out var sortOrderValue) && Enum.TryParse<SortOrder>(sortOrderValue, true, out var parsedSortOrder))
        {
            return parsedSortOrder;
        }

        return DefaultSortOrder;
    }

    private SortColumn GetSortColumnFromQueryString()
    {
        if (ViewContext.HttpContext.Request.Query.TryGetValue("SortColumn", out var sortColumn) && Enum.TryParse<SortColumn>(sortColumn, true, out var parsedSortColumn))
        {
            return parsedSortColumn;
        }

        return SortColumn.Default;
    }

    private string GetSearchTermFromQueryString()
    {
        if (ViewContext.HttpContext.Request.Query.ContainsKey("SearchTerm"))
        {
            return ViewContext.HttpContext.Request.Query["SearchTerm"];
        }

        return string.Empty;
    }
}

public enum SortColumnType
{
    Text,
    Date,
    Amount
}
