using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers
{
    public class CheckboxTagHelper : TagHelper
    {
        private const string ItemClass = "govuk-checkboxes__item";
        private const string InputClass = "govuk-checkboxes__input";
        private const string LabelClass = "govuk-label govuk-checkboxes__label";
        private const string DescriptionClass = "govuk-hint govuk-checkboxes__hint";

        [HtmlAttributeName("asp-for")]
        public ModelExpression Property { get; set; }

        [HtmlAttributeName("label")]
        public string Label { get; set; }
        
        [HtmlAttributeName("hint")]
        public string Hint { get; set; }

        [HtmlAttributeName("aria-controls")]
        public string DataAriaControls { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("css-class")]
        public string CssClass { get; set; } = "govuk-checkboxes";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";

            var innerContent = await output.GetChildContentAsync();

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

                if (attemptedValue == null)
                {
                    attemptedValue = Property.Model as List<string>;
                }
            }

            var isChecked = attemptedValue != null && attemptedValue.Contains("true");
            var checkedValue = isChecked ? " checked " : "";
            var dataAriaControlsValue = string.IsNullOrWhiteSpace(DataAriaControls) ? "" : $" data-aria-controls=\"{DataAriaControls}\" ";

            var id = Property.Name;

            content.Append($"<div class=\"{ItemClass}\">");

            content.Append($"<input id=\"{id}\" type = \"checkbox\"{checkedValue}class=\"{InputClass}\" name=\"{Property.Name}\"{dataAriaControlsValue}value=\"true\">");

            content.Append($"<label class=\"{LabelClass}\" for=\"{id}\">{Label}</label>");

            if (!string.IsNullOrWhiteSpace(Hint))
            {
                if (!string.IsNullOrWhiteSpace(Hint))
                {
                    content.Append($"<span class=\"{DescriptionClass}\" for=\"{id}\">{ Hint}</span>");
                }
            }

            content.Append("</div>");

            content.Append(innerContent.GetContent());

            content.Append("</div>");

            output.Content.SetHtmlContent(content.ToString());
            output.Attributes.Clear();

            await base.ProcessAsync(context, output);
        }
    }
}
