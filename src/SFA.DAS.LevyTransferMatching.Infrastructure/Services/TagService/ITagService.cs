using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService
{
    public interface ITagService
    {
        Task<List<ReferenceDataItem>> GetLevels();
        Task<List<ReferenceDataItem>> GetSectors();
        Task<List<ReferenceDataItem>> GetJobRoles();
    }
}
