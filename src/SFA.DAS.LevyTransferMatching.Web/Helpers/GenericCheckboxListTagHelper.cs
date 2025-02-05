using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.LevyTransferMatching.Web.Models.Shared;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers;

public class GenericCheckboxListTagHelper : TagHelper
{
    private const string ItemClass = "govuk-checkboxes__item";
    private const string InputClass = "govuk-checkboxes__input";
    private const string LabelClass = "govuk-label govuk-checkboxes__label";
    private const string DescriptionClass = "govuk-hint govuk-checkboxes__hint";

    [HtmlAttributeName("asp-for")]
    public ModelExpression Property { get; set; }

    [HtmlAttributeName("source")]
    public IEnumerable<CheckboxListItem> Source { get; set; }

    [HtmlAttributeName("show-description")]
    public bool ShowHint { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    [HtmlAttributeName("css-class")]
    public string CssClass { get; set; } = "govuk-checkboxes";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "";
        output.Attributes.Clear();

        var content = new StringBuilder();
        content.Append($"<div class=\"{CssClass}\">");

        List<int> attemptedValue = null;

        if (ViewContext.ModelState.TryGetValue(Property.Name, out var modelStateEntry))
        {
            if (!string.IsNullOrWhiteSpace(modelStateEntry.AttemptedValue))
            {
                attemptedValue = modelStateEntry.AttemptedValue.Split(",").ToList().ConvertAll(int.Parse);
            }
        }
        else
        {
            attemptedValue = Property.Model as List<int>;
        }

        var index = 0;
        foreach (var tag in Source)
        {
            index++;
            var isChecked = attemptedValue != null && attemptedValue.Contains(tag.Id);
            var checkedValue = isChecked ? " checked " : "";

            var id = index == 1 ? Property.Name : $"{Property.Name}-{index}";

            content.Append($"<div class=\"{ItemClass}\">");

            content.Append($"<input id=\"{id}\" type = \"checkbox\"{checkedValue}class=\"{InputClass}\" name=\"{Property.Name}\" value=\"{tag.Id}\">");

            content.Append($"<label class=\"{LabelClass}\" for=\"{id}\">{tag.Label}</label>");

            if (ShowHint)
            {
                if (!string.IsNullOrWhiteSpace(tag.Hint))
                {
                    content.Append($"<span class=\"{DescriptionClass}\" for=\"{id}\">{ tag.Hint}</span>");
                }
            }

            content.Append("</div>");
        }
        content.Append("</div>");

        output.PostContent.SetHtmlContent(content.ToString());
    }
}