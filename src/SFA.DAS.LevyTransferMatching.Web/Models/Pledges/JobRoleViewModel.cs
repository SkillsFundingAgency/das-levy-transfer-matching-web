using System.Collections.Generic;
using SFA.DAS.LevyTransferMatching.Infrastructure.Tags;

namespace SFA.DAS.LevyTransferMatching.Web.Models.Pledges
{
    public class JobRoleViewModel : JobRolePostRequest
    {
        public List<Tag> JobRoleOptions { get; set; }
    }
}
