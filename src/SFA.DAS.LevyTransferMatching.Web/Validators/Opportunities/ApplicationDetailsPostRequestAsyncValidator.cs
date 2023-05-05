using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;

[assembly: InternalsVisibleTo("SFA.DAS.LevyTransferMatching.Web.UnitTests")]
namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    internal class ApplicationDetailsPostRequestAsyncValidator : AsyncValidator<ApplicationDetailsPostRequest>
    {
        private const string ExceedsAvailableFundingError = "Cost of training exceeds the amount remaining in this pledge";
        private const string StartDateError = "Start date must be between {0} and {1}";

        public ApplicationDetailsPostRequestAsyncValidator(IOpportunitiesService opportunitiesService)
        {
            RuleFor(request => request.SelectedStandardId)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Enter a valid job role")
                .NotEmpty().WithMessage("Enter a valid job role")
            ;

            RuleFor(request => request.StartDate)
                .NotNull()
                .WithMessage(string.Format(StartDateError, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDisplayString(), DateTime.Now.FinancialYearEnd().ToShortDisplayString()))
                .InclusiveBetween(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.FinancialYearEnd())
                .WithMessage(string.Format(StartDateError, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDisplayString(), DateTime.Now.FinancialYearEnd().ToShortDisplayString()))
            ;

            RuleFor(request => request.NumberOfApprentices)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("You must enter the number of apprentices")
                .NotEmpty().WithMessage("You must enter the number of apprentices")
                .Must((request, s) => request.ParsedNumberOfApprentices.HasValue && request.ParsedNumberOfApprentices.Value > 0)
                .WithMessage("You must enter the number of apprentices")                
            ;

            RuleFor(request => request.ExceedsAvailableFunding)
               .MustAsync(async (model, numberOfApprentices, cancellation) =>
               {
                   var result = await opportunitiesService.GetApplicationDetails(model.AccountId, model.PledgeId, model.SelectedStandardId);
                   var selectedStandard = result.Standards.First();

                   if (selectedStandard.ApprenticeshipFunding == null || !selectedStandard.ApprenticeshipFunding.Any())
                   {
                       return false;
                   }

                   return !model.StartDate.HasValue || result.Opportunity.RemainingAmount >= selectedStandard.ApprenticeshipFunding
                       .GetEffectiveFundingLine(model.StartDate.Value)
                       .CalcFundingForDate(model.ParsedNumberOfApprentices, model.StartDate.Value);
               })
               .WithMessage(ExceedsAvailableFundingError)
           ;

            RuleFor(request => request.HasTrainingProvider)
                .NotNull()
                .WithMessage("You must select whether or not you have found a training provider")
            ;
        }
    }
}