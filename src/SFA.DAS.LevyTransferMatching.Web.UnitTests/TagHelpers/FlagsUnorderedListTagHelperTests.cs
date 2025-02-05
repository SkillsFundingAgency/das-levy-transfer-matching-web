using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;
using SFA.DAS.LevyTransferMatching.Web.Helpers;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.TagHelpers;

[TestFixture]
public class FlagsUnorderedListTagHelperTests
{
    private UnorderedListTagHelper _tagHelper;
    private TagHelperContext _tagHelperContext;
    private TagHelperOutput _tagHelperOutput;
    private Mock<IModelMetadataProvider> _modelMetadataProvider;
    private Mock<ICompositeMetadataDetailsProvider> _compositeMetadataDetailsProvider;
    private DefaultMetadataDetails _defaultMetadataDetails;
    private List<ReferenceDataItem> _tagHelperSource;

    [SetUp]
    public void Setup()
    {
        _tagHelper = new UnorderedListTagHelper();

        _tagHelperSource =
        [
            new() { Id = "Option1", Description = "Option one" },
            new() { Id = "Option2", Description = "Option two" },
            new() { Id = "Option3", Description = "Option three" }
        ];

        _tagHelperContext = new TagHelperContext(
            [],
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));

        _tagHelperOutput = new TagHelperOutput("div",
            [],
            (_, _) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        _modelMetadataProvider = new Mock<IModelMetadataProvider>();
        _compositeMetadataDetailsProvider = new Mock<ICompositeMetadataDetailsProvider>();

        var t = typeof(TestClass);
        var propertyInfo = t.GetProperty("TestProperty");

        _defaultMetadataDetails = new DefaultMetadataDetails(ModelMetadataIdentity.ForProperty(propertyInfo,
                typeof(string),
                typeof(TestClass)),
            ModelAttributes.GetAttributesForProperty(typeof(TestClass),
                propertyInfo));
    }

    [TestCase("", "<ul></ul>")]
    [TestCase("Option1", "<ul><li>Option one</li></ul>")]
    [TestCase("Option2", "<ul><li>Option two</li></ul>")]
    [TestCase("Option3", "<ul><li>Option three</li></ul>")]
    [TestCase("Option1,Option2", "<ul><li>Option one</li><li>Option two</li></ul>")]
    public void Process_Renders_Expected_Elements(string propertyValue, string expectedOutput)
    {
        var model = new TestClass { TestProperty = propertyValue.Split(",").ToList() };
        var modelExplorer = new ModelExplorer(_modelMetadataProvider.Object, new DefaultModelMetadata(_modelMetadataProvider.Object, _compositeMetadataDetailsProvider.Object, _defaultMetadataDetails), model.TestProperty);
        _tagHelper.Property = new ModelExpression("TestProperty", modelExplorer);
        _tagHelper.Source = _tagHelperSource;

        _tagHelper.Process(_tagHelperContext, _tagHelperOutput);

        var output = _tagHelperOutput.PostContent;

        output.GetContent().Should().Be(expectedOutput);
    }

    private class TestClass
    {
        public List<string> TestProperty { get; set; }
    }
}