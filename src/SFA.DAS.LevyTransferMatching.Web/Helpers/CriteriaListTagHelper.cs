using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers
{
    public class CriteriaListTagHelper : TagHelper
    {
        [HtmlAttributeName("application")]
        public ApplicationsViewModel.Application Application { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = new StringBuilder();
            var matchesList = new StringBuilder();
            int matches = 0;
            if (Application.IsLocationMatch)
            {
                matches++;
                matchesList.Append("<li>Location</li>");
            }
            if (Application.IsSectorMatch)
            {
                matches++;
                matchesList.Append("<li>Sector</li>");
            }
            if (Application.IsJobRoleMatch)
            {
                matches++;
                matchesList.Append("<li>Job Role</li>");
            }
            if (Application.IsLevelMatch)
            {
                matches++;
                matchesList.Append("<li>Level</li>");
            }

            string matchPercentageText = "";
            switch (matches)
            {
                case 0: matchPercentageText = "0% match"; break;
                case 1: matchPercentageText = "25% match"; break;
                case 2: matchPercentageText = "50% match"; break;
                case 3: matchPercentageText = "75% match"; break;
                case 4: matchPercentageText = "100% match"; break;
            }

            content.Append("<p class=\"govuk-!-margin-bottom-1\">" + matchPercentageText + "</p>");

            if (matches > 0)
            {
                content.Append("<ul class=\"app-criteria-list govuk-list govuk-list--bullet\">");
                content.Append(matchesList.ToString());
                content.Append("</ul>");
            }

            output.PostContent.SetHtmlContent(content.ToString());
            output.Attributes.Clear();
        }
    }
}
