using System.Globalization;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions;

public static class DoubleExtensions
{
    public static int ToNearest(this double value, int roundTo)
    {
        return (int)Math.Round(value / roundTo, MidpointRounding.AwayFromZero) * roundTo;
    }

    public static string ToCurrencyString(this double amount)
    {
        return amount.ToString("C0", new CultureInfo("en-GB"));
    }
}