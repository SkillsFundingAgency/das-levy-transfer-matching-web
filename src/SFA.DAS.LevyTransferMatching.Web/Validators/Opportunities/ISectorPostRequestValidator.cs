using FluentValidation.Results;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    public interface ISectorPostRequestValidator
    {
        ValidationResult Validate(SectorPostRequest sectorPostRequest);
    }
}
