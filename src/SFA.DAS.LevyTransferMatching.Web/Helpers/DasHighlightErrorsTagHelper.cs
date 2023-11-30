using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq.Expressions;
using System.Text.Encodings.Web;



namespace SFA.DAS.LevyTransferMatching.Web.Helpers
{
    [HtmlTargetElement("div", Attributes = HighlightErrorForAttributeName + "," + ErrorCssClass)]
    [HtmlTargetElement("textarea", Attributes = HighlightErrorForAttributeName + "," + ErrorCssClass)]
    [HtmlTargetElement("input", Attributes = HighlightErrorForAttributeName + "," + ErrorCssClass)]
    public class DasHighlightErrorsTagHelper : TagHelper
    {
        private const string HighlightErrorForAttributeName = "das-highlight-error-for";
        private const string ErrorCssClass = "error-class";

        [HtmlAttributeName(HighlightErrorForAttributeName)]
        public ModelExpression Property { get; set; }

        [HtmlAttributeName(ErrorCssClass)]
        public string CssClass { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!ViewContext.ModelState.ContainsKey(Property.Name)) return;

            var modelState = ViewContext.ModelState[Property.Name];

            if (modelState.Errors.Count == 0) return;
            output.AddClass(CssClass, HtmlEncoder.Default);
        }
    }

    public static class HtmlHelperExtensions
    {
        public static HtmlString AddClassIfPropertyInError<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string errorClass)
        {
            var expressionProvider = htmlHelper.ViewContext.HttpContext.RequestServices
                .GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;

            var expressionText = expressionProvider?.GetExpressionText(expression);
            var fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var state = htmlHelper.ViewData.ModelState[fullHtmlFieldName];

            if (state?.Errors == null || state.Errors.Count == 0)
            {
                return HtmlString.Empty;
            }

            return new HtmlString(errorClass);
        }
    }
}
