using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers;

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

        var matchPercentageText = matches switch
        {
            0 => "0% match",
            1 => "25% match",
            2 => "50% match",
            3 => "75% match",
            4 => "100% match",
            _ => ""
        };

        content.Append("<p class=\"govuk-!-margin-bottom-1\">" + matchPercentageText + "</p>");

        if (matches is > 0 and < 4)
        {
            content.Append("<ul class=\"app-criteria-list govuk-list govuk-list--bullet\">");
            content.Append(matchesList.ToString());
            content.Append("</ul>");
        }

        output.PostContent.SetHtmlContent(content.ToString());
        output.Attributes.Clear();
    }
}