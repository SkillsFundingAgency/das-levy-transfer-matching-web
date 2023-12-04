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

    [Test]
    public void Process_Renders_Expected_Elements()
    {
        List<Tuple<string, string>> _tagHelperSource = new List<Tuple<string, string>>
        {
            new Tuple<string, string>( "http://localhost/one", "One" ),
            new Tuple<string, string>( "http://localhost/two", "Two" ),
            new Tuple<string, string>( "http://localhost/three", "Three" )
        };
        var expectedOutput = $"<div class=\"govuk-breadcrumbs\"><ol class=\"govuk-breadcrumbs__list\"><li class=\"govuk-breadcrumbs__list-item\"><a class=\"govuk-breadcrumbs__link\" href=\"{_tagHelperSource[0].Item1}\">{_tagHelperSource[0].Item2}</a></li><li class=\"govuk-breadcrumbs__list-item\"><a class=\"govuk-breadcrumbs__link\" href=\"{_tagHelperSource[1].Item1}\">{_tagHelperSource[1].Item2}</a></li><li class=\"govuk-breadcrumbs__list-item\">{_tagHelperSource[2].Item2}</li></ol></div>";
        _tagHelper.Source = _tagHelperSource;

        _tagHelper.Process(_tagHelperContext, _tagHelperOutput);

        Assert.That(_tagHelperOutput.PostContent.GetContent(), Is.EqualTo(expectedOutput));
    }
}