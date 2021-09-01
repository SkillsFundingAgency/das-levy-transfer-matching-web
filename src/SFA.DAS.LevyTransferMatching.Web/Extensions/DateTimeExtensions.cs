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
            return dateTime.Month >= 4 ? new DateTime(dateTime.Year + 1, 4, 5) : new DateTime(dateTime.Year, 4, 5);
        }

        public static DateTime EndOfMarchOfFinancialYear(this DateTime dateTime)
        {
            return dateTime.Month >= 4 ? new DateTime(dateTime.Year + 1, 3, 31) : new DateTime(dateTime.Year, 3, 31);
        }

        public static string ToShortDisplayString(this DateTime dateTime)
        {
            return dateTime.ToString("MM/yyyy");
        }

        public static int MonthsTillFinancialYearEnd(this DateTime date)
        {
            var financialYear = FinancialYearEnd(date);
            return ((financialYear.Year - date.Year) * 12) + financialYear.Month - date.Month;
        }

        public static bool IsNull(this DateTime? date)
        {
            return !date.HasValue || date == new DateTime(1900, 1, 1);
        }

        public static string ToLongDateDisplayString(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

    }
}