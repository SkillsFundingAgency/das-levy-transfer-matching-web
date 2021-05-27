using System;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers
{
    public class FlagsUnorderedListTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression Property { get; set; }

        [HtmlAttributeName("class")]
        public string Class { get; set; }

        [HtmlAttributeName("item-class")]
        public string ItemClass { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";

            var content = new StringBuilder();
            content.Append($"<ul class=\"{Class}\">");

            var zeroValue = Enum.Parse(Property.Model.GetType(), "0");

            var modelValue = Property.Model as Enum;

            foreach (Enum enumValue in Enum.GetValues(Property.Metadata.ModelType))
            {
                if (enumValue.Equals(zeroValue)) continue;
                var isChecked = modelValue != null && modelValue.HasFlag(enumValue);
                if (isChecked)
                {
                    content.Append($"<li class=\"{ItemClass}\">{enumValue.GetDisplayName()}");
                    content.Append("</li>");
                }
            }
            content.Append("</ul>");

            output.PostContent.SetHtmlContent(content.ToString());
            output.Attributes.Clear();
        }
    }
}
