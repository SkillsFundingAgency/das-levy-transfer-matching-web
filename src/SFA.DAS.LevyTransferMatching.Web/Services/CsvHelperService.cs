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

                AddProperty(record, "Pledge Id", application.EncodedPledgeId);
                AddProperty(record, "Application Id", application.EncodedApplicationId);
                AddProperty(record, "Date Applied", application.DateApplied);
                AddProperty(record, "Application Status", application.Status);
                AddProperty(record, "Employer Name", application.EmployerAccountName);
                
                AddLocationColumns(application, record, totalLocationColumnsRequired);

                AddProperty(record, "Location Match", application.IsLocationMatch.ToYesNo());
                AddProperty(record, "Sectors", application.FormattedSectors);
                AddProperty(record, "Sector Match",application.IsSectorMatch.ToYesNo());
                AddProperty(record, "Job Role", application.TypeOfJobRole);
                AddProperty(record, "Job Role Match", application.IsJobRoleMatch.ToYesNo());
                AddProperty(record, "Level", application.Level);
                AddProperty(record, "Level Match", application.IsLevelMatch.ToYesNo());
                AddProperty(record, "Number of apprentices", application.NumberOfApprentices);
                AddProperty(record, "Start By", application.StartBy);
                AddProperty(record, "Have a training provider", application.HasTrainingProvider.ToYesNo());
                AddProperty(record, "About", application.AboutOpportunity);
                AddProperty(record, "Duration (Months)", application.Duration);
                AddProperty(record, "Total Estimated Cost", application.TotalEstimatedCost);
                AddProperty(record, "Estimated cost this year", application.EstimatedCostThisYear);
                AddProperty(record, "Contact Name", application.ContactName);
                AddProperty(record, "Email Addresses", application.FormattedEmailAddress );
                AddProperty(record, "Business Website", application.BusinessWebsite); 

                listOfRecords.Add(record);
            }

            return listOfRecords;
        }

        public static void AddLocationColumns(PledgeApplicationDownloadModel application, dynamic record, int totalLocationColumnsRequired)
        {
            if(totalLocationColumnsRequired == 0)
            {
                return;
            }

            if(application?.DynamicLocations?.Count() == 0 && totalLocationColumnsRequired > 0)
            {
                for (int i = 0; i < totalLocationColumnsRequired; i++)
                {
                    var fieldName = $"Location{i + 1}";
                    AddProperty(record, fieldName, string.Empty);
                }
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