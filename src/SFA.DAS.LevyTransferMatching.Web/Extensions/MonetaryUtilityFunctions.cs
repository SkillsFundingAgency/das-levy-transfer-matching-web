using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public class MonetaryUtilityFunctions
    {
        public static int CalculateEstimatedTotalCost(int amount, int numberOfApprentices)
        {
            return amount * numberOfApprentices;
        }
    }
}
