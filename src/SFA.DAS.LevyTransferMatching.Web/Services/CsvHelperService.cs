using System.Dynamic;
using System.Globalization;
using CsvHelper;
using SFA.DAS.LevyTransferMatching.Web.Extensions;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Services;

public interface ICsvHelperService
{
    byte[] GenerateCsvFileFromModel(PledgeApplicationsDownloadModel model);
}

public class CsvHelperService : ICsvHelperService
{
    public byte[] GenerateCsvFileFromModel(PledgeApplicationsDownloadModel model)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer,  new CultureInfo("en-GB"));

        var records = SetUpColumnsOnCsv(model);
        csv.WriteRecords(records);
          
        csv.Flush();

        return memoryStream.ToArray();
    }

    private static List<dynamic> SetUpColumnsOnCsv(PledgeApplicationsDownloadModel model)
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

            AddProperty(record, "Location Match", application.MatchLocation.ToYesNo());
            AddProperty(record, "Sectors", application.FormattedSectors);
            AddProperty(record, "Sector Match",application.MatchSector.ToYesNo());
            AddProperty(record, "Job Role", application.TypeOfJobRole);
            AddProperty(record, "Job Role Match", application.MatchJobRole.ToYesNo());
            AddProperty(record, "Level", application.Level);
            AddProperty(record, "Level Match", application.MatchLevel.ToYesNo());
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
        if (totalLocationColumnsRequired == 0)
        {
            return;
        }

        if (application?.DynamicLocations?.Count() == 0 && totalLocationColumnsRequired > 0)
        {
            for (var index = 0; index < totalLocationColumnsRequired; index++)
            {
                var fieldName = $"Location{index + 1}";
                AddProperty(record, fieldName, string.Empty);
            }
            return;
        }

        for (var index = 0; index < application?.DynamicLocations?.Count(); index++)
        {
            var fieldName = $"Location{index + 1}";

            AddProperty(record, fieldName, application.DynamicLocations.ElementAt(index).Name);
        }

        var columnsNeeded = totalLocationColumnsRequired - application?.DynamicLocations?.Count();

        for (var index = application?.DynamicLocations?.Count() ?? 0; index <= columnsNeeded; index++)
        {
            var fieldName = $"Location{index + 1}";
            AddProperty(record, fieldName, string.Empty);
        }
    }

    private static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
    {
        // ExpandoObject supports IDictionary so we can extend it like this
        var expandoDict = expando as IDictionary<string, object>;
        if (expandoDict.ContainsKey(propertyName))
            expandoDict[propertyName] = propertyValue;
        else
            expandoDict.Add(propertyName, propertyValue);
    }
}