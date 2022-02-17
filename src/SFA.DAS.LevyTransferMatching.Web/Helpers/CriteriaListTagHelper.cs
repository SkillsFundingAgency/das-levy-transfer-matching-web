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
        public ApplicationViewModel Application { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = new StringBuilder();
            var matchesList = new StringBuilder();

            if (Application.MatchLocation)
            {
                matchesList.Append("<li>Location</li>");
            }
            if (Application.MatchSector)
            {
                matchesList.Append("<li>Sector</li>");
            }
            if (Application.MatchJobRole)
            {
                matchesList.Append("<li>Job Role</li>");
            }
            if (Application.MatchLevel)
            {
                matchesList.Append("<li>Level</li>");
            }

            content.Append("<p class=\"govuk-!-margin-bottom-1\">" + Application.MatchPercentage + "% match</p>");

            if (Application.MatchPercentage > 0)
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
