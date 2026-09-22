using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace TSFramework.Core.Utils
{
    public class EDateTime
    {
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var localZone = TimeZone.CurrentTimeZone;
            var beginDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return localZone.ToLocalTime(beginDate).AddMilliseconds(timestamp);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            var localZone = TimeZone.CurrentTimeZone;
            var currentDate = DateTime.Now;
            var beginDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            var diff = localZone.ToLocalTime(currentDate) - localZone.ToLocalTime(beginDate);
            return Math.Floor(diff.TotalMilliseconds);
        }

        public static List<ListItem> CreateListMonth()
        {
            return new List<ListItem>
            {
                new ListItem {Text = "Jan", Value = "1"},
                new ListItem {Text = "Feb", Value = "2"},
                new ListItem {Text = "Mar", Value = "3"},
                new ListItem {Text = "Apr", Value = "4"},
                new ListItem {Text = "May", Value = "5"},
                new ListItem {Text = "Jun", Value = "6"},
                new ListItem {Text = "Jul", Value = "7"},
                new ListItem {Text = "Aug", Value = "8"},
                new ListItem {Text = "Sep", Value = "9"},
                new ListItem {Text = "Oct", Value = "10"},
                new ListItem {Text = "Nov", Value = "11"},
                new ListItem {Text = "Dec", Value = "12"}
            };
        }

        public static List<ListItem> CreateListYear(int beginYear = 2000, int endYear = 2020)
        {
            var listYears = new List<ListItem>();
            for (var year = beginYear; year <= endYear; year++)
                listYears.Add(new ListItem {Text = year.ToString(), Value = year.ToString()});

            return listYears;
        }

        public static int MonthDifference(DateTime startDate, DateTime endDate)
        {
            var iMonthDifference = endDate.Month - startDate.Month + (endDate.Year - startDate.Year) * 12;
            return iMonthDifference;
        }
    }
}