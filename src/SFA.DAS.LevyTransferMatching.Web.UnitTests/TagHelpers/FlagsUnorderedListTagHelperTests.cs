﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using NUnit.Framework;
using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;
using SFA.DAS.LevyTransferMatching.Web.Helpers;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.TagHelpers
{
    [TestFixture]
    public class FlagsUnorderedListTagHelperTests
    {
        private UnorderedListTagHelper _tagHelper;
        private TagHelperContext _tagHelperContext;
        private TagHelperOutput _tagHelperOutput;
        private Mock<IModelMetadataProvider> _modelMetadataProvider;
        private Mock<ICompositeMetadataDetailsProvider> _compositeMetadataDetailsProvider;
        private DefaultMetadataDetails _defaultMetadataDetails;
        private List<Tag> _tagSource;

        [SetUp]
        public void Setup()
        {
            _tagHelper = new UnorderedListTagHelper();

            _tagSource = new List<Tag>
            {
                new Tag { TagId = "Option1", Description = "Option one" },
                new Tag { TagId = "Option2", Description = "Option two" },
                new Tag { TagId = "Option3", Description = "Option three" }
            };

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
            _tagHelper.Source = _tagSource;

            _tagHelper.Process(_tagHelperContext, _tagHelperOutput);

            var output = _tagHelperOutput.PostContent;

            Assert.AreEqual(expectedOutput, output.GetContent());
        }

        public class TestClass
        {
            public List<string> TestProperty { get; set; }
        }


    }
}
