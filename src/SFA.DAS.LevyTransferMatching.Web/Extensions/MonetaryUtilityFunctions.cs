namespace SFA.DAS.LevyTransferMatching.Web.Extensions;

public class MonetaryUtilityFunctions
{
    public static int CalculateEstimatedTotalCost(int amount, int numberOfApprentices)
    {
        return amount * numberOfApprentices;
    }
}