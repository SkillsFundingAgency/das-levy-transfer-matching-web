using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class BoolExtensions
    {
        public static string ToApplyViewString(this bool? value)
        {
            return value.HasValue ? value.Value ? "Yes" : "No" : "-";
        }
    }
}
