using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.CacheStorage;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.OpportunitiesService.Types;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.PledgeService.Types;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;
using SFA.DAS.LevyTransferMatching.Web.Services;

namespace SFA.DAS.LevyTransferMatching.Web.Orchestrators.Pledge
{
    public interface IDownloadApplicationsOrchestrator
    {
        Task<byte[]> GetPledgeApplicationsDownloadModel(ApplicationsRequest request);
    }
    
    public class DownloadApplicationsOrchestrator : PledgeBaseOrchestrator, IDownloadApplicationsOrchestrator
    {
        private readonly ICsvHelperService _csvService;
        private readonly IEncodingService _encodingService;

        public DownloadApplicationsOrchestrator(ICacheStorageService cacheStorageService,
            IPledgeService pledgeService,
            ICsvHelperService csvService,
            IEncodingService encodingService)
            : base(cacheStorageService, pledgeService)
        {
            _csvService = csvService;
            _encodingService = encodingService;
        }

        public async Task<byte[]> GetPledgeApplicationsDownloadModel(ApplicationsRequest request)
        {
            var result = await PledgeService.GetApplications(request.AccountId, request.PledgeId);

            var pledgeAppModel = new PledgeApplicationsDownloadModel
            {
                Applications = result.Applications?.Select(app => new PledgeApplicationDownloadModel
                {
                    DateApplied = app.CreatedOn,
                    Status = app.Status,
                    ApplicationId = app.Id,
                    PledgeId = app.PledgeId,
                    EmployerAccountName = app.DasAccountName,
                    HasTrainingProvider = app.HasTrainingProvider,
                    Sectors = app.Sectors ?? new List<string>(),
                    AboutOpportunity = app.Details,
                    BusinessWebsite = app.BusinessWebsite?.WithUrlPrefix(),
                    FormattedEmailAddress = string.Join(";", app.EmailAddresses),
                    FormattedSectors = string.Join(",", app.Sectors ?? new List<string>()),
                    FirstName = app.FirstName,
                    LastName = app.LastName,
                    NumberOfApprentices = app.NumberOfApprentices,
                    StartBy = app.StartDate,
                    TypeOfJobRole = app.JobRole,
                    EncodedPledgeId = _encodingService.Encode(app.PledgeId, EncodingType.PledgeId),
                    EncodedApplicationId = _encodingService.Encode(app.Id, EncodingType.PledgeApplicationId),
                    IsJobRoleMatch = app.IsJobRoleMatch,
                    IsLevelMatch = app.IsLevelMatch,
                    IsLocationMatch = app.IsLocationMatch,
                    IsSectorMatch = app.IsSectorMatch,
                    Duration = app.StandardDuration,
                    EstimatedCostThisYear = app.Amount,
                    Level = app.Level,
                    TotalEstimatedCost = app.MaxFunding,
                    AdditionalLocations = app.AdditionalLocations,
                    SpecificLocation = app.SpecificLocation,
                    Locations = app.Locations,
                    PledgeLocations = app.PledgeLocations,
                    DynamicLocations = GetListOfLocations(app.PledgeLocations, app.Locations, app.SpecificLocation, app.AdditionalLocations)
                })
            };

            var fileContents = _csvService.GenerateCsvFileFromModel(pledgeAppModel);

            return fileContents;
        }

        private IEnumerable<dynamic> GetListOfLocations(IEnumerable<GetApplyResponse.PledgeLocation> pledgeLocations, IEnumerable<GetApplicationsResponse.ApplicationLocation> applicationLocations, string specificLocation, string additionalLocations)
        {
            var listOfMatchingLocations = (from location in applicationLocations
                                           select pledgeLocations?.FirstOrDefault(o => o.Id == location.PledgeLocationId)
                into matchedLocation
                                           where matchedLocation != null && !string.IsNullOrWhiteSpace(matchedLocation.Name)
                                           select matchedLocation.Name).ToList();

            if (!string.IsNullOrWhiteSpace(specificLocation))
            {
                listOfMatchingLocations.Add(specificLocation);
            }

            if (!string.IsNullOrWhiteSpace(additionalLocations))
            {
                listOfMatchingLocations.Add(additionalLocations);
            }

            var locations = new List<dynamic>();

            foreach (var matchingLocation in listOfMatchingLocations)
            {
                dynamic location = new ExpandoObject();
                location.Name = matchingLocation;
                locations.Add(location);
            }

            return locations;
        }
    }
}
