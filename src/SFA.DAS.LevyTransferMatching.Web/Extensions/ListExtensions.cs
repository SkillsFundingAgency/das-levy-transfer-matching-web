using System.Collections.Generic;
using System.Linq;
using SFA.DAS.LevyTransferMatching.Infrastructure.ReferenceData;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class ListExtensions
    {
        public static bool IsComplete(this List<string> list)
        {
            return list != null && list.Any();
        }
    }
}
