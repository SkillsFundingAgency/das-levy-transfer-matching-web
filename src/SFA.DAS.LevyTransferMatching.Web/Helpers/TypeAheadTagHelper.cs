using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers;

public class TypeAheadTagHelper : TagHelper
{
    [HtmlAttributeName("asp-for")]
    public ModelExpression Property { get; set; }

    [HtmlAttributeName("source")]
    public IEnumerable<StandardsListItemViewModel> Source { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = string.Empty;

        var content = new StringBuilder();
        content.Append($"<select id=\"{Property.Name}\" name=\"{Property.Name}\" class=\"govuk-select\">");
        content.Append($"<option value={string.Empty}></option>");

        foreach (var standard in Source)
        {
            var selected = standard.Selected;

            if (!ViewContext.ModelState.IsValid && ViewContext.ModelState["SelectedStandardId"] != null)
            {
                selected = ViewContext.ModelState["SelectedStandardId"]?.AttemptedValue == string.Empty
                    ? null
                    : standard.Id == ViewContext.ModelState["SelectedStandardId"]?.AttemptedValue
                        ? "selected"
                        : null;
            }

            content.Append($"<option value=\"{standard.Id}\" {(string.IsNullOrEmpty(selected) ? string.Empty : "selected=\"selected\"")}>{standard.Title}</option>");
        }
        content.Append("</select>");

        output.PostContent.SetHtmlContent(content.ToString());
        output.Attributes.Clear();
    }
}