using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers
{
    public class CheckboxListTagHelper : TagHelper
    {
        private const string ItemClass = "govuk-checkboxes__item";
        private const string InputClass = "govuk-checkboxes__input";
        private const string LabelClass = "govuk-label govuk-checkboxes__label";
        private const string DescriptionClass = "govuk-hint govuk-checkboxes__hint";
        private const string CssClass = "govuk-checkboxes govuk-checkboxes--large";

        [HtmlAttributeName("asp-for")]
        public ModelExpression Property { get; set; }

        [HtmlAttributeName("source")]
        public List<Tag> Source { get; set; }

        [HtmlAttributeName("show-description")]
        public bool ShowDescription { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";

            var content = new StringBuilder();
            content.Append($"<div class=\"{CssClass}\">");

            List<string> attemptedValue = null;

            if (ViewContext.ModelState.ContainsKey(Property.Name))
            {
                  var modelStateEntry = ViewContext.ModelState[Property.Name];
                if (!string.IsNullOrWhiteSpace(modelStateEntry.AttemptedValue))
                {
                    attemptedValue = modelStateEntry.AttemptedValue.Split(",").ToList();
                }
            }

            if (attemptedValue == null)
            {
                attemptedValue = Property.Model as List<string>;
            }

            var i = 0;
            foreach (var tag in Source)
            {
                i++;
                var isChecked = attemptedValue != null && attemptedValue.Contains(tag.TagId);
                var checkedValue = isChecked ? " checked " : "";

                var id = i == 1 ? Property.Name : $"{Property.Name}-{i}";

                content.Append($"<div class=\"{ItemClass}\">");

                content.Append($"<input id=\"{id}\" type = \"checkbox\"{checkedValue}class=\"{InputClass}\" name=\"{Property.Name}\" value=\"{tag.TagId}\">");

                content.Append($"<label class=\"{LabelClass}\" for=\"{id}\">{tag.Description}</label>");

                if (ShowDescription)
                {
                    if (!string.IsNullOrWhiteSpace(tag.ExtendedDescription))
                    {
                        content.Append($"<span class=\"{DescriptionClass}\" for=\"{id}\">{ tag.ExtendedDescription}</span>");
                    }
                }

                content.Append("</div>");
            }
            content.Append("</div>");

            output.PostContent.SetHtmlContent(content.ToString());
            output.Attributes.Clear();
        }
    }
}
