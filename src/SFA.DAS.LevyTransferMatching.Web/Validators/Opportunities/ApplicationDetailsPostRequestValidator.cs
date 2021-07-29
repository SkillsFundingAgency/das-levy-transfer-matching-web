using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    public class ApplicationDetailsPostRequestValidator : AbstractValidator<ApplicationDetailsPostRequest>
    {
        private const int MinApprentices = 1;
        private const string NumApprenticesError = "There is not enough funding to support this many apprentices";
        private const string StartDateError = "Start date must be between {0} and {1}";        

        public ApplicationDetailsPostRequestValidator(IOpportunitiesService opportunitiesService)
        {
            RuleFor(request => request.SelectedStandardId)
                .NotNull()
                .WithMessage("Enter a valid job role")
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
                .GreaterThan(0).WithMessage("You must enter the number of apprentices")
                .MustAsync(async (model, numberOfApprentices, cancellation) =>
                {
                    if (!model.StartDate.HasValue)
                    {
                        return false;
                    }

                    var result = await opportunitiesService.GetApplicationDetails(model.PledgeId, model.SelectedStandardId);
                    var selectedStandard = result.Standards.First();

                    if (selectedStandard.ApprenticeshipFunding == null || !selectedStandard.ApprenticeshipFunding.Any())
                    {
                        return false;
                    }

                    var funding = selectedStandard.ApprenticeshipFunding
                        .FirstOrDefault(c =>
                            c.EffectiveFrom <= model.StartDate
                            && (c.EffectiveTo.IsNull() || c.EffectiveTo >= model.StartDate)) ??
                                  selectedStandard.ApprenticeshipFunding.First(c => c.EffectiveTo.IsNull());

                    return result.Opportunity.Amount >= CurrentYearFunding(numberOfApprentices.Value, funding.MaxEmployerLevyCap, funding.Duration, model.StartDate.Value);
                })
                .WithMessage(NumApprenticesError)
            ;

            RuleFor(request => request.HasTrainingProvider)
                .NotNull()
                .WithMessage("You must select whether or not you have found a training provider")
            ;
        }

        private static int MaxFunding(int? numberOfApprentices, int maxEmployerLevyCap) => (numberOfApprentices ?? 0) * maxEmployerLevyCap;

        private static int CurrentYearFunding(int? numberOfApprentices, int maxEmployerLevyCap, int duration, DateTime startDate) => (MaxFunding(numberOfApprentices, maxEmployerLevyCap) / duration) * startDate.MonthsTillFinancialYearEnd();
    }
}