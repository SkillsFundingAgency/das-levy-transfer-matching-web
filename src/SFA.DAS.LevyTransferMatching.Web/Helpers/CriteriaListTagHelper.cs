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
            int matches = 0;
            if (Application.IsLocationMatch)
            {
                matches++;
                matchesList.Append("• Location <br>");
            }
            if (Application.IsSectorMatch)
            {
                matches++;
                matchesList.Append("• Sector <br>");
            }
            if (Application.IsJobRoleMatch)
            {
                matches++;
                matchesList.Append("• Job Role <br>");
            }
            if (Application.IsLevelMatch)
            {
                matches++;
                matchesList.Append("• Level <br>");
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

            content.Append("<span>" + matchPercentageText + "</span>");

            if (matches > 0)
            {
                content.Append("<p style=\"color: grey; padding-top: 5px; font-size: 16px;\">");
                content.Append(matchesList.ToString());
                content.Append("</p>");
            }

            output.PostContent.SetHtmlContent(content.ToString());
            output.Attributes.Clear();
        }
    }
}
