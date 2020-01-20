using System;

namespace Ced.Utility
{
    public class DateHelper
    {
        public static string GetDisplayDate(DateTime? startDate, DateTime? endDate)
        {
            var startYear = startDate.GetValueOrDefault().Year;
            var startMonth = startDate.GetValueOrDefault().Month;
            var startDay = startDate.GetValueOrDefault().Day.ToString().PadLeft(2, '0');

            var endYear = endDate.GetValueOrDefault().Year;
            var endMonth = endDate.GetValueOrDefault().Month;
            var endDay = endDate.GetValueOrDefault().Day.ToString().PadLeft(2, '0');

            var startMonthName = startDate.GetValueOrDefault().ToString("MMM");
            var endMonthName = endDate.GetValueOrDefault().ToString("MMM");

            if (startYear == endYear)
            {
                if (startMonth == endMonth)
                {
                    if (startDay == endDay)
                        return startDay + " " + startMonthName + " " + endYear;
                    return startDay + " - " + endDay + " " + startMonthName + " " + endYear;
                }

                return startDay + " " + startMonthName + " - " + endDay + " " + endMonthName + " " + endYear;
            }

            return startDay + " " + startMonthName + startYear + " - " + endDay + " " + endMonthName + " " + endYear;
        }
    }
}