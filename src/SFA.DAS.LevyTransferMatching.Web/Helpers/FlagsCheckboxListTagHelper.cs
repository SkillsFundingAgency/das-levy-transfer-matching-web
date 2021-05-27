using System;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.LevyTransferMatching.Web.Extensions;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers
{
    public class FlagsCheckboxListTagHelper : TagHelper
    {
        private const string ItemClass = "govuk-checkboxes__item";
        private const string InputClass = "govuk-checkboxes__input";
        private const string LabelClass = "govuk-label govuk-checkboxes__label";
        private const string DescriptionClass = "govuk-hint govuk-checkboxes__hint";
        private const string CssClass = "govuk-checkboxes govuk-checkboxes--large";

        [HtmlAttributeName("asp-for")]
        public ModelExpression Property { get; set; }

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

            var zeroValue = Enum.Parse(Property.Model.GetType(), "0");

            Enum attemptedValue = null;

            if (ViewContext.ModelState.ContainsKey(Property.Name))
            {
                var modelStateEntry = ViewContext.ModelState[Property.Name];
                if (!string.IsNullOrWhiteSpace(modelStateEntry.AttemptedValue))
                {
                    attemptedValue = Enum.Parse(Property.Model.GetType(), modelStateEntry.AttemptedValue) as Enum;
                }
            }

            if (attemptedValue == null)
            {
                attemptedValue = Property.Model as Enum;
            }


            var i = 0;
            foreach (Enum enumValue in Enum.GetValues(Property.Metadata.ModelType))
            {
                if (enumValue.Equals(zeroValue)) continue;
                i++;
                var isChecked = attemptedValue != null && attemptedValue.HasFlag(enumValue);
                var checkedValue = isChecked ? " checked " : "";

                var id = i == 1 ? Property.Name : $"{Property.Name}-{i}";

                content.Append($"<div class=\"{ItemClass}\">");

                content.Append($"<input id=\"{id}\" type = \"checkbox\"{checkedValue}class=\"{InputClass}\" name=\"{Property.Name}\" value=\"{enumValue}\">");

                content.Append($"<label class=\"{LabelClass}\" for=\"{id}\">{enumValue.GetDisplayName()}</label>");

                if (ShowDescription)
                {
                    var description = enumValue.GetDescription();
                    if (!String.IsNullOrWhiteSpace(description))
                    {
                        content.Append($"<span class=\"{DescriptionClass}\" for=\"{id}\">{description}</span>");
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
