using System.Globalization;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions;

public static class IntegerExtensions
{
    public static string ToCurrencyString(this int amount)
    {
        return amount.ToString("C0", new CultureInfo("en-GB"));
    }
}