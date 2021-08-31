using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class IntExtensions
    {
        public static string MatchPercentageCssClass(this int matchPercentage)
        {
            return matchPercentage switch
            {
                100 => "green",
                75 => "yellow",
                _ => "red"
            };
        }
    }
}
