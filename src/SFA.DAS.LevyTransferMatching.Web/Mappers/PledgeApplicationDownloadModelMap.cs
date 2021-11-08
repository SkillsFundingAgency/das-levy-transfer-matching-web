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
            Map(m => m.FormattedSectors).Name("Sectors");
            Map(m => m.TypeOfJobRole).Name("Job Role");
            Map(m => m.Level);
            Map(m => m.NumberOfApprentices).Name("Number of apprentices");
            Map(m => m.StartBy).Name("Start By");
            Map(m => m.HasTrainingProvider).Name("Have a training provider").TypeConverter<BooleanConverter>();
            Map(m => m.AboutOpportunity).Name("About");
            Map(m => m.ContactName).Name("Contact Name");
            Map(m => m.FormattedEmailAddress).Name("Contact Email");
            Map(m => m.BusinessWebsite).Name("Contact Website");
            Map(m => m.AdditionalLocation).Ignore();
            Map(m => m.SpecificLocation).Ignore();
        }
    }
}
