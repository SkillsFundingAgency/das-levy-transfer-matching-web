using System;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToTaxYear(this DateTime dateTime, string format)
        {
            DateTime newTaxYear = new DateTime(dateTime.Year, 4, 6);

            if (dateTime.Date < newTaxYear)
            {
                dateTime = dateTime.AddYears(-1);
            }

            return dateTime.ToString(format);
        }
    }
}