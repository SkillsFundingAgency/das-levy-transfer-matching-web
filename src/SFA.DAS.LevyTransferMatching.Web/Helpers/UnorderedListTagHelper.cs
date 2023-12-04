using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers;

public class UnorderedListTagHelper : TagHelper
{
    [HtmlAttributeName("asp-for")]
    public ModelExpression Property { get; set; }

    [HtmlAttributeName("source")]
    public List<ReferenceDataItem> Source { get; set; }

    [HtmlAttributeName("class")]
    public string Class { get; set; }

    [HtmlAttributeName("item-class")]
    public string ItemClass { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "";

        var content = new StringBuilder();

        content.Append(!string.IsNullOrWhiteSpace(Class) ? $"<ul class=\"{Class}\">" : $"<ul>");

        var modelValue = Property.Model as List<string>;

        foreach (var tag in Source)
        {
            var isChecked = modelValue != null && modelValue.Contains(tag.Id);
            if (!isChecked)
            {
                continue;
            }
            
            content.Append("<li");
            
            if (!string.IsNullOrWhiteSpace(ItemClass))
            {
                content.Append($" class=\"{ItemClass}\"");
            }
            content.Append('>');
            content.Append($"{tag.Description}");
            content.Append("</li>");
        }
        content.Append("</ul>");

        output.PostContent.SetHtmlContent(content.ToString());
        output.Attributes.Clear();
    }
}