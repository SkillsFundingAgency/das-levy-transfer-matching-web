using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace SFA.DAS.LevyTransferMatching.Web.Helpers
{
    public class TileViewTagHelper : TagHelper
    {
        [HtmlAttributeName("first-tile-text")]
        public string FirstTileText { get; set; }

        [HtmlAttributeName("first-tile-heading")]
        public string FirstTileHeading { get; set; }

        [HtmlAttributeName("first-tile-css-class")]
        public string FirstTileCssClass { get; set; } = "";

        [HtmlAttributeName("second-tile-text")]
        public string SecondTileText { get; set; }

        [HtmlAttributeName("second-tile-heading")]
        public string SecondTileHeading { get; set; }

        [HtmlAttributeName("second-tile-css-class")]
        public string SecondTileCssClass { get; set; } = "app-data--information";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = new StringBuilder();
            content.Append(GetHTML(FirstTileText, FirstTileHeading, FirstTileCssClass))
                .Append(GetHTML(SecondTileText, SecondTileHeading, SecondTileCssClass));
            output.PostContent.SetHtmlContent(content.ToString());
            output.Attributes.Clear();
        }

        private string GetHTML(string text, string heading, string cssClass)
        {
            return "<div class=\"govuk-grid-column-one-third\"><div class=\"app-data " + cssClass
                + "\"><p class=\"govuk-heading-s app-data__title\">" + text
                + "</p><p class=\"govuk-heading-l app-data__figure\">" + heading + "</p></div></div>";
        }
    }
}
