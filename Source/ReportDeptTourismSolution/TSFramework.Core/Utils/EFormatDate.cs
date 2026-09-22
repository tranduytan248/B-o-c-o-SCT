using System;
using System.Collections.Generic;
using System.Globalization;

namespace TSFramework.Core.Utils
{
    public static class EFormatDate
    {
        #region DateType enum

        public enum DateType
        {
            ddMMyyyy = 1,
            MMddyyyy = 2,
            yyyyMMdd = 3,
            MMyyyy = 4
        }

        #endregion

        /// <summary>
        ///     Convert2s the string.
        /// </summary>
        /// <returns></returns>
        public static string Convert2String()
        {
            var datestring = string.Empty;
            datestring += DateTime.Now.Year.ToString();
            datestring += DateTime.Now.Month.ToString();
            datestring += DateTime.Now.Day.ToString();
            datestring += DateTime.Now.Hour.ToString();
            datestring += DateTime.Now.Minute.ToString();
            datestring += DateTime.Now.Second.ToString();
            return datestring;
        }

        /// <summary>
        ///     Converts the hour string.
        /// </summary>
        /// <returns></returns>
        public static string ConvertHourString()
        {
            var datestring = string.Empty;

            datestring += DateTime.Now.Hour.ToString();
            datestring += DateTime.Now.Minute.ToString();
            datestring += DateTime.Now.Second.ToString();
            return datestring;
        }

        /// <summary>
        ///     Converts date from dd/MM/yyyy to yyyy/MM/dd.
        /// </summary>
        /// <returns>DateTime with format yyyy/MM/dd</returns>
        public static DateTime YYYYMMDD(string date, DateType type)
        {
            string[] strdate;

            if (!string.IsNullOrEmpty(date))
                strdate = date.Split('/');
            else
                return DateTime.Now;

            var dt = new DateTime();
            switch (type)
            {
                case DateType.ddMMyyyy:
                    dt =
                        Convert.ToDateTime(new DateTime(Convert.ToInt32(strdate[2]), Convert.ToInt32(strdate[1]),
                            Convert.ToInt32(strdate[0])));
                    break;
                case DateType.MMddyyyy:
                    dt =
                        Convert.ToDateTime(new DateTime(Convert.ToInt32(strdate[2]), Convert.ToInt32(strdate[0]),
                            Convert.ToInt32(strdate[1])));
                    break;
                case DateType.yyyyMMdd:
                    dt =
                        Convert.ToDateTime(new DateTime(Convert.ToInt32(strdate[0]), Convert.ToInt32(strdate[1]),
                            Convert.ToInt32(strdate[2])));
                    break;
                case DateType.MMyyyy:
                    dt = Convert.ToDateTime(new DateTime(Convert.ToInt32(strdate[1]), Convert.ToInt32(strdate[0]), 1));
                    break;
            }

            return dt;
        }

        /// <summary>
        ///     Converts the date.
        /// </summary>
        /// <returns></returns>
        public static string ConvertDate()
        {
            var datestring = string.Empty;

            datestring += DateTime.Now.Day + "/";
            datestring += DateTime.Now.Month + "/";
            datestring += DateTime.Now.Year.ToString();

            return string.Format(datestring, "dd/MM/yyyy");
        }

        /// <summary>
        ///     Converts the date VN.
        /// </summary>
        /// <returns></returns>
        public static string ConvertDayToVN(DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Thứ hai";
                case DayOfWeek.Tuesday:
                    return "Thứ ba";
                case DayOfWeek.Wednesday:
                    return "Thứ tư";
                case DayOfWeek.Thursday:
                    return "Thứ năm";
                case DayOfWeek.Friday:
                    return "Thứ sáu";
                case DayOfWeek.Saturday:
                    return "Thứ bảy";
                case DayOfWeek.Sunday:
                    return "Chủ nhật";
                default:
                    return "";
            }
        }

        /// <summary>
        ///     Get number week of year for culture vi-VN.
        /// </summary>
        /// <returns> Number week to int type</returns>
        public static int GetWeekByDate(DateTime date)
        {
            var cul = new CultureInfo("vi-VN");
            var dfi = cul.DateTimeFormat;
            var cal = dfi.Calendar;
            return cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
        }

        /// <summary>
        ///     Get first day of week.
        /// </summary>
        /// <returns> First Day of week to DateTime</returns>
        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            var cul = new CultureInfo("vi-VN");
            var dfi = cul.DateTimeFormat;
            var cal = dfi.Calendar;
            var jan1 = new DateTime(year, 1, 1);

            var daysOffset = (int) dfi.FirstDayOfWeek - (int) jan1.DayOfWeek;

            var firstMonday = daysOffset < 0 ? jan1.AddDays(daysOffset + 7) : jan1.AddDays(daysOffset);

            var firstWeek = GetWeekByDate(jan1);

            if (daysOffset == 0)
                return firstMonday.AddDays((weekOfYear - 1) * 7);

            if (firstWeek == 0) return firstMonday.AddDays(weekOfYear * 7);
            return firstMonday.AddDays((weekOfYear - 2) * 7);
        }

        /// <summary>
        ///     Get last day of week
        /// </summary>
        /// <returns> DateTime - Last day of week</returns>
        public static DateTime LastDayOfWeek(int year, int weekOfyear)
        {
            return FirstDateOfWeek(year, weekOfyear).AddDays(6);
        }

        /// <summary>
        ///     Get day list in week
        /// </summary>
        /// <returns> List<DateTime> - day list in week</returns>
        public static List<DateTime> GetDayInWeek(int year, int weekOfYear)
        {
            var list = new List<DateTime>();
            var firstMonday = FirstDateOfWeek(year, weekOfYear);

            for (var i = 0; i < 7; i++)
            {
                DateTime date;
                date = firstMonday;
                date = date.AddDays(i);
                if (date.Year == year)
                    list.Add(date);
            }

            return list;
        }
    }
}