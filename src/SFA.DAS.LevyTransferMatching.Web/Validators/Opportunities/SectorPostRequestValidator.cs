using FluentValidation;
using FluentValidation.Results;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities
{
    public class SectorPostRequestValidator : ISectorPostRequestValidator
    {
        public ValidationResult Validate(SectorPostRequest sectorPostRequest)
        {
            var validationResult = new ValidationResult();
            Regex regex = new Regex(@"^[A-Za-z]{1,2}\d[A-Za-z\d]?\s*\d[A-Za-z]{2}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (sectorPostRequest.Sectors == null || !sectorPostRequest.Sectors.Any())
            {
                validationResult.Errors.Add(new ValidationFailure(nameof(sectorPostRequest.Sectors), "Select one or more business sectors to describe your business"));
            }

            if(sectorPostRequest.Postcode == null || sectorPostRequest.Postcode == "" || !regex.IsMatch(sectorPostRequest.Postcode))
            {
                validationResult.Errors.Add(new ValidationFailure(nameof(sectorPostRequest.Postcode), "Enter a valid postcode"));
            }

            return validationResult;
        }
    }
}
