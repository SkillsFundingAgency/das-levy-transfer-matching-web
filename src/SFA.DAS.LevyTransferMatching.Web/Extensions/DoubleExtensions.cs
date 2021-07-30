using System;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class DoubleExtensions
    {
        public static double ToNearest(this double value, int roundTo)
        {
            return Math.Round(value / roundTo, MidpointRounding.AwayFromZero) * roundTo;
        }
    }
}
