using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    public class ApplicationDetailsPostRequestValidator : AbstractValidator<ApplicationDetailsPostRequest>
    {
        private const int MinApprentices = 1;
        private const int MaxApprentices = 100; // Temporary max, will be altered by TM-36
        private const string DateError = "Enter a valid date";
        private const string NumApprenticesError = "The number of apprentices must be between {0} and {1}";

        public ApplicationDetailsPostRequestValidator()
        {
            RuleFor(request => request.JobRole)
                .NotEmpty()
                .WithMessage("Enter a valid job role")
            ;

            RuleFor(request => request.NumberOfApprentices)
                .NotNull()
                .WithMessage(string.Format(NumApprenticesError, MinApprentices, MaxApprentices))
                .InclusiveBetween(MinApprentices, MaxApprentices)
                .WithMessage(string.Format(NumApprenticesError, MinApprentices, MaxApprentices))
            ;

            RuleFor(request => request.Month)
                .NotNull()
                .WithMessage(DateError)
                .InclusiveBetween(1, 12)
                .WithMessage(DateError)
            ;

            RuleFor(request => request.Year)
                .NotNull()
                .WithMessage(DateError)
            ;

            RuleFor(request => request.StartDate)
                .NotNull()
                .WithMessage(DateError)
            ;

            RuleFor(request => request.HasTrainingProvider)
                .NotNull()
                .WithMessage("You must select either yes or no")
            ;
        }
    }
}
