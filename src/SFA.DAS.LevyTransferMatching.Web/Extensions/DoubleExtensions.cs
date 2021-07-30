using System;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class DoubleExtensions
    {
        public static int ToNearest(this double value, int roundTo)
        {
            return (int)Math.Round(value / roundTo, MidpointRounding.AwayFromZero) * roundTo;
        }
    }
}
