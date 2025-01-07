using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.LevyTransferMatching.Web.Helpers;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.TagHelpers;

public class BreadCrumbTagHelperTests
{
    private BreadCrumbTagHelper _tagHelper;
    private TagHelperContext _tagHelperContext;
    private TagHelperOutput _tagHelperOutput;

    [SetUp]
    public void Setup()
    {
        _tagHelper = new BreadCrumbTagHelper();
           

        _tagHelperContext = new TagHelperContext(
            [],
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));

        _tagHelperOutput = new TagHelperOutput("div",
            [],
            (result, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });
    }

    [Test]
    public void Process_Renders_Expected_Elements()
    {
        var tagHelperSource = new List<Tuple<string, string>>
        {
            new( "http://localhost/one", "One" ),
            new( "http://localhost/two", "Two" ),
            new( "http://localhost/three", "Three" )
        };
        
        var expectedOutput = $"<div class=\"govuk-breadcrumbs\"><ol class=\"govuk-breadcrumbs__list\"><li class=\"govuk-breadcrumbs__list-item\"><a class=\"govuk-breadcrumbs__link\" href=\"{tagHelperSource[0].Item1}\">{tagHelperSource[0].Item2}</a></li><li class=\"govuk-breadcrumbs__list-item\"><a class=\"govuk-breadcrumbs__link\" href=\"{tagHelperSource[1].Item1}\">{tagHelperSource[1].Item2}</a></li><li class=\"govuk-breadcrumbs__list-item\">{tagHelperSource[2].Item2}</li></ol></div>";
        _tagHelper.Source = tagHelperSource;

        _tagHelper.Process(_tagHelperContext, _tagHelperOutput);

        _tagHelperOutput.PostContent.GetContent().Should().Be(expectedOutput);
    }
}