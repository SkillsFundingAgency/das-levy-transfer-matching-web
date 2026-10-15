using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.LevyTransferMatching.Domain.Types;
using SFA.DAS.LevyTransferMatching.Web.Helpers;

namespace SFA.DAS.LevyTransferMatching.Web.UnitTests.TagHelpers;

public class SortableColumnTagHelperTests
{
    [TestCase(SortColumnType.Text, SortOrder.Ascending, SortColumn.Applicant, "Applicant", "Sort Applicant Z to A")]
    [TestCase(SortColumnType.Text, SortOrder.Descending, SortColumn.Applicant, "Applicant", "Sort Applicant A to Z")]
    [TestCase(SortColumnType.Date, SortOrder.Ascending, SortColumn.ApplicationDate, "ApplicationDate", "Sort ApplicationDate newest to oldest")]
    [TestCase(SortColumnType.Date, SortOrder.Descending, SortColumn.ApplicationDate, "ApplicationDate", "Sort ApplicationDate oldest to newest")]
    [TestCase(SortColumnType.Amount, SortOrder.Ascending, SortColumn.CurrentFinancialYearAmount, "CurrentFinancialYearAmount", "Sort CurrentFinancialYearAmount high to low")]
    [TestCase(SortColumnType.Amount, SortOrder.Descending, SortColumn.CurrentFinancialYearAmount, "CurrentFinancialYearAmount", "Sort CurrentFinancialYearAmount low to high")]
    public void Process_ShouldGenerateAccessibleLinkWithCorrectAttributes(SortColumnType columnType, SortOrder sortOrder, SortColumn sortColumn, string label, string expectedAriaLabel)
    {
        var urlHelper = new Mock<IUrlHelper>();

        urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).
            Returns("/items");

        var urlHelperFactory = new Mock<IUrlHelperFactory>();
        urlHelperFactory.Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>())).Returns(urlHelper.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.QueryString = new QueryString($"?SortColumn={label}&SortOrder={sortOrder}");

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        var viewContext = new ViewContext
        {
            HttpContext = httpContext,
            RouteData = actionContext.RouteData,
        };

        // Arrange
        var tagHelper = new SortableColumnTagHelper(urlHelperFactory.Object, new Mock<IActionContextAccessor>().Object)
        {
            ViewContext = viewContext,
            ColumnType = columnType,
            ColumnName = sortColumn,
            Label = label,
            IsDefault = false,
            DefaultSortOrder = SortOrder.Ascending
        };
        var context = new TagHelperContext(
            [],
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput("sortable-column",
            new TagHelperAttributeList(),
            (result, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });
        // Act
        tagHelper.Process(context, output);
        // Assert
        output.Attributes.Should().ContainSingle(a => a.Name == "href");
        output.Attributes.Should().ContainSingle(a => a.Name == "class" && a.Value.ToString().Contains("govuk-link"));
        output.Attributes["href"].Value.ToString().Should().Be("/items");
        output.Attributes["aria-label"].Value.ToString().Should().Be(expectedAriaLabel);
        output.Content.GetContent().Should().Be(label);
    }
}