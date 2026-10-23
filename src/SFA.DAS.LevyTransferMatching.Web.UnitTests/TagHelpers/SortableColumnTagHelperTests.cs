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
    private UrlActionContext _urlActionContext;

    [TestCase(SortColumnType.Text, SortOrder.Ascending, "ascending", "A to Z")]
    [TestCase(SortColumnType.Text, SortOrder.Descending, "descending", "Z to A")]
    [TestCase(SortColumnType.Date, SortOrder.Ascending, "ascending", "oldest to newest")]
    [TestCase(SortColumnType.Date, SortOrder.Descending, "descending", "newest to oldest")]
    [TestCase(SortColumnType.Amount, SortOrder.Ascending, "ascending", "low to high")]
    [TestCase(SortColumnType.Amount, SortOrder.Descending, "descending", "high to low")]
    public void Process_ActiveColumn_ExposesCurrentSortStateOnHeaderAndLink(SortColumnType columnType, SortOrder sortOrder, string expectedAriaSort, string expectedDirectionText)
    {
        var output = Process(SortColumn.Applicant, columnType, $"?SortColumn=Applicant&SortOrder={sortOrder}");

        output.TagName.Should().Be("th");
        output.Attributes["scope"].Value.ToString().Should().Be("col");
        output.Attributes["aria-sort"].Value.ToString().Should().Be(expectedAriaSort);
        output.Content.GetContent().Should().Contain($"<span class=\"govuk-visually-hidden\">, sorted {expectedDirectionText}</span>");
    }

    [TestCase(SortOrder.Ascending, "Descending")]
    [TestCase(SortOrder.Descending, "Ascending")]
    public void Process_ActiveColumn_LinksToReversedSortOrder(SortOrder sortOrder, string expectedNextSortOrder)
    {
        Process(SortColumn.Applicant, SortColumnType.Text, $"?SortColumn=Applicant&SortOrder={sortOrder}");

        new RouteValueDictionary(_urlActionContext.Values)["SortOrder"].Should().Be(expectedNextSortOrder);
    }

    [Test]
    public void Process_InactiveColumn_HasNoSortState()
    {
        var output = Process(SortColumn.Status, SortColumnType.Text, "?SortColumn=Applicant&SortOrder=Ascending");

        output.Attributes.ContainsName("aria-sort").Should().BeFalse();
        output.Content.GetContent().Should().NotContain("govuk-visually-hidden");
        output.Content.GetContent().Should().Contain("class=\"govuk-link das-table__sort\"");
    }

    [Test]
    public void Process_LinkTargetsItsOwnHeaderSoFocusIsRetainedWithoutJavaScript()
    {
        var output = Process(SortColumn.Applicant, SortColumnType.Text, string.Empty);

        _urlActionContext.Fragment.Should().Be("sort-Applicant");
        output.Content.GetContent().Should().Contain("id=\"sort-Applicant\"");
        output.Content.GetContent().Should().Contain("data-sort-column=\"Applicant\"");
    }

    [Test]
    public void Process_AppendsHeaderClass()
    {
        var output = Process(SortColumn.Applicant, SortColumnType.Text, string.Empty, "das-table-cell-width-25");

        output.Attributes["class"].Value.ToString().Should().Be("govuk-table__header das-table-cell-width-25");
    }

    private TagHelperOutput Process(SortColumn columnName, SortColumnType columnType, string queryString, string headerClass = null)
    {
        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
            .Callback<UrlActionContext>(ctx => _urlActionContext = ctx)
            .Returns("/items");

        var urlHelperFactory = new Mock<IUrlHelperFactory>();
        urlHelperFactory.Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>())).Returns(urlHelper.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.QueryString = new QueryString(queryString);

        var viewContext = new ViewContext
        {
            HttpContext = httpContext,
            RouteData = new RouteData()
        };

        var tagHelper = new SortableColumnTagHelper(urlHelperFactory.Object, new Mock<IActionContextAccessor>().Object)
        {
            ViewContext = viewContext,
            ColumnType = columnType,
            ColumnName = columnName,
            Label = columnName.ToString(),
            HeaderClass = headerClass,
            IsDefault = false,
            DefaultSortOrder = SortOrder.Ascending
        };

        var context = new TagHelperContext([], new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
        var output = new TagHelperOutput("sortable-column", new TagHelperAttributeList(),
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        tagHelper.Process(context, output);

        return output;
    }
}
