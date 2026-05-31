using System;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Helpers
{
    public static class ConvertDateTime
    {
        public static int GetCurrentUnixTimeStamp()
            => (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

        public static DateTime GetFirstDayOfMonth(DateTime date)
            => new(date.Year, date.Month, 1, 0, 0, 0);

        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            var firstDay = GetFirstDayOfMonth(date);
            return firstDay.AddMonths(1).AddSeconds(-1);
        }

        public static int DateTimeToUnixTimeStamp(DateTime dt)
        {
            var dateTimeOffset = new DateTimeOffset(dt).ToUniversalTime();
            return (int)dateTimeOffset.ToUnixTimeSeconds();
        }

        public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            // System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            // dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            var dtDateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).ToLocalTime().DateTime;
            return dtDateTime;
        }

        public static DateTime GetStartOfTheDay(DateTime date) =>
            new(date.Year, date.Month, date.Day, 0, 0, 0);

        public static DateTime GetEndOfTheDay(DateTime date) =>
            new(date.Year, date.Month, date.Day, 23, 59, 59);

        public static int HourToCurrentDate(DateTime currentDate, int hour) =>
            DateTimeToUnixTimeStamp(GetStartOfTheDay(currentDate)) + hour;

        /// <summary>
        /// Tính số tuổi(năm) đến thời điểm hiện tại
        /// </summary>
        /// <param name="dateOfBirth">ngày sinh</param>
        /// lcliem 13/7/2021
        public static double CalculateAge(DateTime dateOfBirth)
        {
            double age = 0;
            var currentDate = DateTime.Now;
            age = currentDate.Year - dateOfBirth.Year;
            var temp = (double)currentDate.DayOfYear / (double)dateOfBirth.DayOfYear;
            if (currentDate.DayOfYear != 1 && dateOfBirth.DayOfYear == 1)
            {
                age = age * 1.001;
            }
            else if (temp > 1)
            {
                age = age + (temp - currentDate.DayOfYear / dateOfBirth.DayOfYear);
            }
            else if (temp < 1)
            {
                age = age - 1 + temp;
            }

            return age;
        }
        /// <summary>
        /// Chuyển đổi định dạng ngày tháng thành chuỗi kí tự
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ConvertDateToString(DateTime? date, [StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string format = "yyyy-MM-dd")
        {
            if (date.HasValue)
            {
                return date.Value.ToString(format);
            }
            return "";
        }
    }
}
