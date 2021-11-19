using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Services
{
    public class CsvHelperService : ICsvHelperService
    {
        public byte[] GenerateCsvFileFromModel(PledgeApplicationsDownloadModel model)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);

            var records = SetUpColumnsOnCsv(model);
            csv.WriteRecords(records);
          
            csv.Flush();

            return memoryStream.ToArray();
        }

        private IEnumerable<dynamic> SetUpColumnsOnCsv(PledgeApplicationsDownloadModel model)
        {
            var listOfRecords = new List<dynamic>();
            var totalLocationColumnsRequired = 0;

            foreach (var appModel in model.Applications)
            {
                if (appModel?.DynamicLocations?.Count() > totalLocationColumnsRequired)
                {
                    totalLocationColumnsRequired = appModel.DynamicLocations.Count();
                }
            }

            foreach (var application in model.Applications)
            {
                dynamic record = new ExpandoObject();

                record.PledgeId = application.EncodedPledgeId;
                record.ApplicationId = application.EncodedApplicationId;
                record.DateApplied = application.DateApplied;
                record.ApplicationStatus = application.Status;
                record.EmployerName = application.EmployerAccountName;
                
                AddLocationColumns(application, record, totalLocationColumnsRequired);

                record.LocationMatch = application.IsLocationMatch.ToYesNo();
                record.Sectors = application.FormattedSectors;
                record.SectorMatch = application.IsSectorMatch.ToYesNo();
                record.JobRole = application.TypeOfJobRole;
                record.JobRoleMatch = application.IsJobRoleMatch.ToYesNo();
                record.Level = application.Level;
                record.LevelMatch = application.IsLevelMatch.ToYesNo();
                record.NumberOfApprentices = application.NumberOfApprentices;
                record.StartBy = application.StartBy;
                record.HaveATrainingProvider = application.HasTrainingProvider.ToYesNo();
                record.About = application.AboutOpportunity;
                record.Duration = application.Duration;
                record.TotalEstimatedCost = application.TotalEstimatedCost;
                record.EstimatedCostThisYear = application.EstimatedCostThisYear;
                record.ContactName = application.ContactName;
                record.EmailAddresses = application.FormattedEmailAddress;
                record.BusinessWebsite = application.BusinessWebsite;

                listOfRecords.Add(record);
            }

            return listOfRecords;
        }

        public static void AddLocationColumns(PledgeApplicationDownloadModel application, dynamic record, int totalLocationColumnsRequired)
        {
            if (totalLocationColumnsRequired == 0)
            {
                return;
            }

            for (int i = 0; i < application?.DynamicLocations?.Count(); i++)
            {
                var fieldName = $"Location{i + 1}";

                AddProperty(record, fieldName, application.DynamicLocations.ElementAt(i).Name);
            }

            var columnsNeeded = totalLocationColumnsRequired - application?.DynamicLocations?.Count();

            for (int i = application?.DynamicLocations?.Count() ?? 0; i <= columnsNeeded; i++)
            {
                var fieldName = $"Location{i + 1}";
                AddProperty(record, fieldName, string.Empty);
            }
        }

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }
}