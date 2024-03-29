﻿using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Pledges
{
    public class LocationSelectPostRequestValidator : AbstractValidator<LocationSelectPostRequest>
    {
        public LocationSelectPostRequestValidator()
        {
            RuleForEach(x => x.SelectValidLocationGroups)
                .Must(x => !string.IsNullOrEmpty(x.SelectedValue))
                .WithMessage("Please select a location");
        }
    }
}