using CsvHelper.Configuration;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Mappers
{
    public sealed class PledgeApplicationDownloadModelMap : ClassMap<PledgeApplicationDownloadModel>
    {
        public PledgeApplicationDownloadModelMap()
        {
            Map(m => m.EncodedPledgeId).Name("Pledge Id");
            Map(m => m.EncodedApplicationId).Name("Application Id");
            Map(m => m.DateApplied).Name("Date Applied");
            Map(m => m.Status).Name("Application Status");
            Map(m => m.EmployerAccountName).Name("Employer Name");
            Map(m => m.FormattedLocations).Name("Locations");
            Map(m => m.IsLocationMatch).Name("Location Match").TypeConverter<BooleanConverter>();
            Map(m => m.FormattedSectors).Name("Sectors");
            Map(m => m.IsSectorMatch).Name("Sector Match").TypeConverter<BooleanConverter>();
            Map(m => m.TypeOfJobRole).Name("Job Role");
            Map(m => m.IsJobRoleMatch).Name("Job Role Match").TypeConverter<BooleanConverter>();
            Map(m => m.Level);
            Map(m => m.IsLevelMatch).Name("Level Match").TypeConverter<BooleanConverter>();
            Map(m => m.NumberOfApprentices).Name("Number of apprentices");
            Map(m => m.StartBy).Name("Start By");
            Map(m => m.HasTrainingProvider).Name("Have a training provider").TypeConverter<BooleanConverter>();
            Map(m => m.AboutOpportunity).Name("About");
            Map(m => m.Duration).Name("Duration (Months)");
            Map(m => m.TotalEstimatedCost).Name("Total estimated cost");
            Map(m => m.EstimatedCostThisYear).Name("Estimated cost this year");
            Map(m => m.ContactName).Name("Contact Name");
            Map(m => m.FormattedEmailAddress).Name("Contact Email");
            Map(m => m.BusinessWebsite).Name("Contact Website");
           
        }
    }
}
