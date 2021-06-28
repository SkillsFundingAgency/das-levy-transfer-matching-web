using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.Services.TagService
{
    public interface ITagService
    {
        Task<List<Tag>> GetLevels();
        Task<List<Tag>> GetSectors();
        Task<List<Tag>> GetJobRoles();
    }
}
