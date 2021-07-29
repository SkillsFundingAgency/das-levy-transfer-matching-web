using FluentValidation;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("SFA.DAS.LevyTransferMatching.Web.UnitTests")]
namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    internal class SectorPostRequestAsyncValidator : AsyncValidator<SectorPostRequest>
    {
        public SectorPostRequestAsyncValidator(IOpportunitiesService opportunitiesService)
        {
            RuleFor(x => x.Sectors)
                .NotNull().WithMessage("Select one or more business sectors to describe your business")
                .NotEmpty().WithMessage("Select one or more business sectors to describe your business");

            var regex = new Regex(@"^[A-Za-z]{1,2}\d[A-Za-z\d]?\s*\d[A-Za-z]{2}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            RuleFor(x => x.Postcode)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Enter a postcode")
                .NotEmpty().WithMessage("Enter a postcode")
                .Matches(regex).WithMessage("Enter a postcode")
                .MustAsync(async (model, postcode, cancellation) =>
                 {
                     var result = await opportunitiesService.GetSector(model.AccountId, model.PledgeId, postcode);
                     return !string.IsNullOrEmpty(result.Location);
                 }).WithMessage("Enter a postcode");
        }
    }
}