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
    private const string CssClass = "govuk-link das-table__sort";

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
        var action = ViewContext.RouteData.Values["action"] as string;
        var controller = ViewContext.RouteData.Values["controller"] as string;

        var sortColumn = GetSortColumnFromQueryString();
        var sortOrder = GetSortOrderFromQueryString();
        var isSortColumn = sortColumn == ColumnName || (sortColumn == SortColumn.Default && IsDefault);

        var nextSortOrder = isSortColumn ? sortOrder.Reverse() : DefaultSortOrder;

        var values = new
        {
            SearchTerm = GetSearchTermFromQueryString(),
            SortColumn = ColumnName,
            SortOrder = isSortColumn ? sortOrder.Reverse().ToString() : DefaultSortOrder.ToString()
        };

        var href = _urlHelper.Action(action, controller, values, null, null, null);

        var sortOrderCssSuffix = string.Empty;
        if (isSortColumn)
        {
            sortOrderCssSuffix = sortOrder == SortOrder.Ascending ? "das-table__sort--asc" : "das-table__sort--desc";
        }

        var directionText = nextSortOrder == SortOrder.Ascending ? "A to Z" : "Z to A";

        output.TagName = "a";
        output.Attributes.SetAttribute("href", href);
        output.Attributes.SetAttribute("class", $"{CssClass} {sortOrderCssSuffix}".Trim());
        output.Attributes.SetAttribute("aria-label",GetSortLabel(nextSortOrder));

        output.Content.SetContent(Label);
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

    private string GetSortLabel(SortOrder nextSortOrder)
    {
        
        var directionText = ColumnType switch
        {
            SortColumnType.Text => nextSortOrder == SortOrder.Ascending ? "A to Z" : "Z to A",
            SortColumnType.Date => nextSortOrder == SortOrder.Ascending ? "oldest to newest" : "newest to oldest",
            SortColumnType.Amount => nextSortOrder == SortOrder.Ascending ? "low to high" : "high to low",
            _ => nextSortOrder == SortOrder.Ascending? "ascending" : "descending",
        };

        return $"Sort {Label} {directionText}";
    }

}

public enum SortColumnType
{
    Text,    
    Date,
    Amount
}