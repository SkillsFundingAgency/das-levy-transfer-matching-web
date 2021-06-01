using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Web.Helpers;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.TagHelpers
{
    [TestFixture]
    public class FlagsUnorderedListTagHelperTests
    {
        private FlagsUnorderedListTagHelper _tagHelper;
        private TagHelperContext _tagHelperContext;
        private TagHelperOutput _tagHelperOutput;
        private Mock<IModelMetadataProvider> _modelMetadataProvider;
        private Mock<ICompositeMetadataDetailsProvider> _compositeMetadataDetailsProvider;
        private DefaultMetadataDetails _defaultMetadataDetails;

        [SetUp]
        public void Setup()
        {
            _tagHelper = new FlagsUnorderedListTagHelper();

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

            _modelMetadataProvider = new Mock<IModelMetadataProvider>();
            _compositeMetadataDetailsProvider = new Mock<ICompositeMetadataDetailsProvider>();

            var t = typeof(TestClass);
            var propertyInfo = t.GetProperty("TestProperty");

            _defaultMetadataDetails = new DefaultMetadataDetails(ModelMetadataIdentity.ForProperty(propertyInfo,
                    typeof(TestFlagsEnum),
                    typeof(TestClass)),
                ModelAttributes.GetAttributesForProperty(typeof(TestClass),
                    propertyInfo));

        }

        [TestCase(TestFlagsEnum.None, "<ul></ul>")]
        [TestCase(TestFlagsEnum.Item1, "<ul><li>Item1</li></ul>")]
        [TestCase(TestFlagsEnum.Item2, "<ul><li>Item2</li></ul>")]
        [TestCase(TestFlagsEnum.Item3, "<ul><li>Item3</li></ul>")]
        [TestCase((TestFlagsEnum)3, "<ul><li>Item1</li><li>Item2</li></ul>")]
        public void Process_Renders_Expected_Elements(TestFlagsEnum propertyValue, string expectedOutput)
        {
            var model = new TestClass { TestProperty = propertyValue };
            var modelExplorer = new ModelExplorer(_modelMetadataProvider.Object, new DefaultModelMetadata(_modelMetadataProvider.Object, _compositeMetadataDetailsProvider.Object, _defaultMetadataDetails), model.TestProperty);
            _tagHelper.Property = new ModelExpression("TestProperty", modelExplorer);

            _tagHelper.Process(_tagHelperContext, _tagHelperOutput);

            var output = _tagHelperOutput.PostContent;

            Assert.AreEqual(expectedOutput, output.GetContent());
        }

        public class TestClass
        {
            public TestFlagsEnum TestProperty { get; set; }
        }

        [Flags]
        public enum TestFlagsEnum
        {
            None = 0,
            Item1 = 1,
            Item2 = 2,
            Item3 = 4
        }
    }
}
