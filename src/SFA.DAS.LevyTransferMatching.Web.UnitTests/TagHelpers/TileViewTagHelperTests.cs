using Microsoft.AspNetCore.Razor.TagHelpers;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Helpers;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.TagHelpers
{
    [TestFixture]
    public class TileViewTagHelperTests
    {
        private TileViewTagHelper _tagHelper;
        private TagHelperContext _tagHelperContext;
        private TagHelperOutput _tagHelperOutput;

        [SetUp]
        public void SetUp()
        {
            _tagHelper = new TileViewTagHelper();

            _tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            _tagHelperOutput = new TagHelperOutput("div",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });
        }

        [TestCase(5_000, 10_000,
            "first-tile-style", "second-tile-style",
            "<div class=\"govuk-grid-column-one-third\"><div class=\"app-data first-tile-style\">" +
            "<p class=\"govuk-heading-s app-data__title\">Pledged Funds for 2021/22</p><p class=\"govuk-heading-l app-data__figure\">£10,000</p></div></div>" + 
            "<div class=\"govuk-grid-column-one-third\"><div class=\"app-data second-tile-style\">" + 
            "<p class=\"govuk-heading-s app-data__title\">Remaining Funds for 2021/22</p><p class=\"govuk-heading-l app-data__figure\">£5,000</p></div></div>")]
        [TestCase(0, 950_000,
            "", "",
            "<div class=\"govuk-grid-column-one-third\"><div class=\"app-data \">" +
            "<p class=\"govuk-heading-s app-data__title\">Pledged Funds for 2021/22</p><p class=\"govuk-heading-l app-data__figure\">£950,000</p></div></div>" +
            "<div class=\"govuk-grid-column-one-third\"><div class=\"app-data \">" +
            "<p class=\"govuk-heading-s app-data__title\">Remaining Funds for 2021/22</p><p class=\"govuk-heading-l app-data__figure\">£0</p></div></div>")]
        public void TagReturnsCorrectHTML(int remainingAmount, int totalAmount, string firstTileCss, string secondTileCss, string expectedOutput)
        {
            _tagHelper.FirstTileText = "Pledged Funds for " + new ApplicationsViewModel().TaxYear;
            _tagHelper.FirstTileHeading = totalAmount.ToString("C0");
            _tagHelper.FirstTileCssClass = firstTileCss;

            _tagHelper.SecondTileText = "Remaining Funds for " + new ApplicationsViewModel().TaxYear;
            _tagHelper.SecondTileHeading = remainingAmount.ToString("C0");
            _tagHelper.SecondTileCssClass = secondTileCss;

            _tagHelper.Process(_tagHelperContext, _tagHelperOutput);

            var output = _tagHelperOutput.PostContent;

            Assert.AreEqual(expectedOutput, output.GetContent());
        }
    }
}
