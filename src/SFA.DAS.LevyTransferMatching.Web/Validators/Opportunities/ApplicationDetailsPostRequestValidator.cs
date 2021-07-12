using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    public class ApplicationDetailsPostRequestValidator : AbstractValidator<ApplicationDetailsPostRequest>
    {
        private const int MinApprentices = 1;
        private const int MaxApprentices = 100; // Temporary max, will be implemented by TM-36
        private const string NumApprenticesError = "The number of apprentices must be between {0} and {1}";
        private const string StartDateError = "Start date must be between {0} and {1}";        

        public ApplicationDetailsPostRequestValidator()
        {
            RuleFor(request => request.SelectedStandardId)
                .NotNull()
                .WithMessage("Enter a valid job role")
            ; 

            RuleFor(request => request.NumberOfApprentices)
                .NotNull()
                .WithMessage(string.Format(NumApprenticesError, MinApprentices, MaxApprentices))
                .InclusiveBetween(MinApprentices, MaxApprentices)
                .WithMessage(string.Format(NumApprenticesError, MinApprentices, MaxApprentices))
            ;

            RuleFor(request => request.StartDate)
                .NotNull()
                .WithMessage(string.Format(StartDateError, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDisplayString(), DateTime.Now.FinancialYearEnd().ToShortDisplayString()))
                .InclusiveBetween(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now.FinancialYearEnd())
                .WithMessage(string.Format(StartDateError, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDisplayString(), DateTime.Now.FinancialYearEnd().ToShortDisplayString()))
            ;

            RuleFor(request => request.HasTrainingProvider)
                .NotNull()
                .WithMessage("You must select either yes or no")
            ;
        }
    }
}
