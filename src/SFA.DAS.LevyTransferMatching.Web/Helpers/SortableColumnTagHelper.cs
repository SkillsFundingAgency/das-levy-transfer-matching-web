using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.LevyTransferMatching.Domain.Extensions;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers
{
    [HtmlTargetElement("sortable-column")]
    public class SortableColumnTagHelper : TagHelper
    {
        private const string CssClass = "govuk-link das-table__sort";

        [HtmlAttributeName("selected-tab")]
        public string SelectedTab { get; set; }

        [HtmlAttributeName("column-name")]
        public SortColumn ColumnName { get; set; }

        [HtmlAttributeName("column-label")]
        public string Label { get; set; }

        [HtmlAttributeName("default-sort-column")]
        public SortColumn DefaultSortColumn { get; set; }

        [HtmlAttributeName("default-order")]
        public SortOrder DefaultSortOrder { get; set; }

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

            var sortColumn = GetColumnFromQueryString();
            var sortOrder = GetSortOrderFromQueryString();
            var isSortColumn = sortColumn == ColumnName;

            var values = new
            {
                SelectedTab = SelectedTab,
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

            var ariaSort = sortOrder.ToString().ToLower();

            var content = new StringBuilder();
            content.Append($"<a class=\"{CssClass} {sortOrderCssSuffix}\" href=\"{href}\" aria-sort=\"{ariaSort}\">");
            content.Append(Label);
            content.Append("</a>");

            output.TagName = "";
            output.PostContent.SetHtmlContent(content.ToString());
            output.Attributes.Clear();
        }

        private SortOrder GetSortOrderFromQueryString()
        {
            if (ViewContext.HttpContext.Request.Query.ContainsKey("SortOrder"))
            {
                if (ViewContext.HttpContext.Request.Query.TryGetValue("SortOrder", out var sortOrderValue))
                {
                    if (Enum.TryParse<SortOrder>(sortOrderValue, true, out var parsedSortOrder))
                    {
                        return parsedSortOrder;
                    }
                }
            }

            return DefaultSortOrder;
        }

        private SortColumn GetColumnFromQueryString()
        {
            if (ViewContext.HttpContext.Request.Query.TryGetValue("SortColumn", out var sortColumn))
            {
                if (Enum.TryParse<SortColumn>(sortColumn, true, out var parsedSortColumn))
                {
                    return parsedSortColumn;
                }
            }

            return DefaultSortColumn;
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
}
