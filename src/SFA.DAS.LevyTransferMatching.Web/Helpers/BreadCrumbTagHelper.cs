using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers
{
    public class BreadCrumbTagHelper : TagHelper
    {
        [HtmlAttributeName("source")]
        public IEnumerable<Tuple<string, string>> Source { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = string.Empty;

            var content = new StringBuilder();
            content.Append($"<div class=\"govuk-breadcrumbs\">");
            content.Append($"<ol class=\"govuk-breadcrumbs__list\">");

            foreach (var crumb in Source)
            {
                content.Append($"<li class=\"govuk-breadcrumbs__list-item\"><a class=\"govuk-breadcrumbs__link\" href=\"{crumb.Item1}\">{crumb.Item2}</a></li>");
            }

            content.Append("</ol>");
            content.Append("</div>");

            output.PostContent.SetHtmlContent(content.ToString());
            output.Attributes.Clear();
        }
    }
}
