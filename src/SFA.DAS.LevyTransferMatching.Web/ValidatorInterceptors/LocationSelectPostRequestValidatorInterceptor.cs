using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.ValidatorInterceptors
{
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