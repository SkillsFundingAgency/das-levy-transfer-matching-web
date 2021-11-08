using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using SFA.DAS.LevyTransferMatching.Web.Mappers;
using SFA.DAS.LevyTransferMatching.Web.Models.Applications;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Services
{
    public class CsvHelperService : ICsvHelperService
    {
        public byte[] GenerateCsvFileFromModel(PledgeApplicationsDownloadModel model)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<PledgeApplicationDownloadModelMap>();

            csv.WriteHeader<PledgeApplicationDownloadModel>();
            csv.NextRecord();

            foreach (var application in model.Applications)
            {
                csv.WriteRecord(application);
                csv.NextRecord();
            }
            csv.Flush();

            return memoryStream.ToArray();
        }
    }
}