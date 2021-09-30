using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Validators.Applications
{
    public class ApplicationStatusPostRequestValidator : AbstractValidator<ApplicationStatusPostRequest>
    {
        public ApplicationStatusPostRequestValidator()
        {
            RuleFor(o => o.HasAcceptedTermsAndConditions)
                .Equal(true)
                .WithMessage("You must agree to the terms and conditions before accepting funding for this application");

            RuleFor(o => o.SelectedAction)
                .Equal(ApplicationPostRequest.ApprovalAction.Approve)
                .WithMessage("You must choose to either accept or decline funding for this application");
        }
    }
}
