﻿using FluentValidation;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Models.Opportunities;
using SFA.DAS.LevyTransferMatching.Web.Validators;
using SFA.DAS.LevyTransferMatching.Web.Validators.Applications;
using SFA.DAS.LevyTransferMatching.Web.Validators.Opportunities;

namespace SFA.DAS.LevyTransferMatching.Web.StartupExtensions;

public static class AddAsyncValidatorsExtensions
{
    public static void AddAsyncValidators(this IServiceCollection services)
    {
        services.AddTransient<AsyncValidator<ApplicationDetailsPostRequest>>(s => new ApplicationDetailsPostRequestAsyncValidator(s.GetService<IOpportunitiesService>()));
        services.AddTransient<AbstractValidator<ApplicationPostRequest>>(s => new ApplicationPostRequestValidator());
    }
}