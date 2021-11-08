using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Web.Models.Pledges;

namespace SFA.DAS.LevyTransferMatching.Web.Services
{
    public interface ICsvHelperService
    {
        byte[] GenerateCsvFileFromModel(PledgeApplicationsDownloadModel model);
    }
}