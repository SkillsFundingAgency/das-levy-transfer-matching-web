using System;

namespace SFA.DAS.LevyTransferMatching.Web.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToTaxYear(this DateTime dateTime, string format)
        {
            DateTime taxYear = dateTime;
            DateTime newTaxYear = new DateTime(taxYear.Year, 4, 6);

            if (taxYear.Date < newTaxYear)
            {
                taxYear = dateTime.AddYears(-1);
            }

            return taxYear.ToString(format);
        }

        public static string ToTaxYearDescription(this DateTime dateTime)
        {
            return $"{dateTime.ToTaxYear("yyyy")}/{dateTime.AddYears(1).ToTaxYear("yy")}";
        }

        public static DateTime FinancialYearEnd(this DateTime dateTime)
        {
            return dateTime.Month >= 4 ? new DateTime(dateTime.Year + 1, 4, 30) : new DateTime(dateTime.Year, 4, 30);
        }        

        public static string ToShortDisplayString(this DateTime dateTime)
        {
            return dateTime.ToString("MM/yyyy");
        }
    }
}