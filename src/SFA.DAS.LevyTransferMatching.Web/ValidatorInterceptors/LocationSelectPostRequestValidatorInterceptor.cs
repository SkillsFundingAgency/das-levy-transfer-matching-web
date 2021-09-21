using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.ValidatorInterceptors
{
    /// <summary>
    /// This interceptor exists because there seems to be an underlying issue
    /// with either the ASP.NET MVC validation or Fluent; (valid) ModelState
    /// entries were not being generated for this validated structure, and
    /// as a result, the ValidationSummary partial wasn't rendering out links
    /// in certain situations, due to the way it loops through on a consecutive
    /// index.
    /// This fix ensures there's always ModelState entries for each value in
    /// the
    /// <see cref="LocationSelectPostRequest.SelectValidLocationGroups" />
    /// property.
    /// </summary>
    public class LocationSelectPostRequestValidatorInterceptor : IValidatorInterceptor
    {
        public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        {
            var postRequest = validationContext.InstanceToValidate as LocationSelectPostRequest;

            var placeholderModelStateValues = new ModelStateDictionary();

            for (var i = 0; i < postRequest.SelectValidLocationGroups.Length; i++)
            {
                placeholderModelStateValues.SetModelValue($"{nameof(LocationSelectPostRequest.SelectValidLocationGroups)}[{i}]", string.Empty, string.Empty);
                placeholderModelStateValues.MarkFieldValid($"{nameof(LocationSelectPostRequest.SelectValidLocationGroups)}[{i}]");
            }

            actionContext.ModelState.Merge(placeholderModelStateValues);

            return result;
        }

        public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
        {
            return commonContext;
        }
    }
}