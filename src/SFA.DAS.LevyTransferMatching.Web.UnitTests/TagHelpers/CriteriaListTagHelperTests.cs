using Microsoft.AspNetCore.Razor.TagHelpers;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Helpers;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.TagHelpers
{
    [TestFixture]
    public class CriteriaListTagHelperTests
    {
        private CriteriaListTagHelper _tagHelper;
        private TagHelperContext _tagHelperContext;
        private TagHelperOutput _tagHelperOutput;

        [SetUp]
        public void SetUp()
        {
            _tagHelper = new CriteriaListTagHelper();

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

        [TestCase(false, false, false, false, "<p class=\"govuk-!-margin-bottom-1\">0% match</p>")]
        [TestCase(true, false, false, false, "<p class=\"govuk-!-margin-bottom-1\">25% match</p><ul class=\"app-criteria-list govuk-list govuk-list--bullet\"><li>Location</li></ul>")]
        [TestCase(false, false, true, false, "<p class=\"govuk-!-margin-bottom-1\">25% match</p><ul class=\"app-criteria-list govuk-list govuk-list--bullet\"><li>Job Role</li></ul>")]
        [TestCase(false, true, true, false, "<p class=\"govuk-!-margin-bottom-1\">50% match</p><ul class=\"app-criteria-list govuk-list govuk-list--bullet\"><li>Sector</li><li>Job Role</li></ul>")]
        [TestCase(true, false, true, true, "<p class=\"govuk-!-margin-bottom-1\">75% match</p><ul class=\"app-criteria-list govuk-list govuk-list--bullet\"><li>Location</li><li>Job Role</li><li>Level</li></ul>")]
        [TestCase(true, true, true, true, "<p class=\"govuk-!-margin-bottom-1\">100% match</p><ul class=\"app-criteria-list govuk-list govuk-list--bullet\"><li>Location</li><li>Sector</li><li>Job Role</li><li>Level</li></ul>")]
        public void ProcessReturnsCorrectHtml(bool isLocationMatch, bool isSectorMatch, bool isJobRoleMatch, bool isLevelMatch, string expectedOutput)
        {
            var application = new ApplicationViewModel
            {
                IsLocationMatch = isLocationMatch,
                IsSectorMatch = isSectorMatch,
                IsJobRoleMatch = isJobRoleMatch,
                IsLevelMatch = isLevelMatch
            };
            
            _tagHelper.Application = application;

            _tagHelper.Process(_tagHelperContext, _tagHelperOutput);

            var output = _tagHelperOutput.PostContent;

            Assert.AreEqual(expectedOutput, output.GetContent());
        }
    }
}
