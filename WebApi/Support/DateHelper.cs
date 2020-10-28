using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WebApi.Support
{
    public static class DateHelper
    {
        ///
        /// Utility method used to display DateTime in a unambiguous layout
        /// <param name="date">The DateTime to be processed</param>
        ///
        public static string DateFormat(this DateTime? Date)
        {
            try
            {
                if (Date.HasValue)
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0:MMM dd, yyyy - hh:mm:ss tt}", Date.Value);
                }
                else return "No Date";
            }
            catch
            {
                return "No Date";
            }
        }

        ///
        /// Utility method used to display DateTime in a unambiguous layout
        /// <param name="date">The DateTime to be processed</param>
        ///
        public static string DateFormat(this DateTime Date) => DateFormat((DateTime?)Date);

        ///
        /// Utility method used to display Time in a unambiguous layout
        /// <param name="date">The DateTime to be processed</param>
        ///
        public static string TimeFormat(this DateTime? Date)
        {
            try
            {
                if (Date.HasValue)
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0:hh:mm:ss tt}", Date.Value);
                }
                else return "No Time";
            }
            catch
            {
                return "No Time";
            }
        }

        ///
        /// Utility method used to display DateTime in a unambiguous layout without time
        /// <param name="date">The DateTime to be processed</param>
        ///
        public static string DateSimpleFormat(this DateTime? Date)
        {
            try
            {
                if (Date.HasValue)
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0:MMM dd, yyyy}", Date.Value);
                }
                else return "No Date";
            }
            catch
            {
                return "No Date";
            }
        }

        ///
        /// Utility method used to display DateTime in a unambiguous layout without time
        /// <param name="date">The DateTime to be processed</param>
        ///
        public static string DateToHHMM(this DateTime? Date)
        {
            try
            {
                if (Date.HasValue)
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0:HH:mm}", Date.Value);
                }
                else return "No Date";
            }
            catch
            {
                return "No Date";
            }
        }

        /// <summary>
        /// Converts a ISO 8601 date string to DateTime
        /// E.g. 2000-01-01T10:00:00Z
        /// </summary>
        /// <param name="IsoDate"></param>
        /// <returns>DateTime object or current Date if a string cannot be parsed</returns>
        public static DateTime FromISO(this string IsoDate)
        {
            try
            {
                return DateTime.Parse(IsoDate, null, DateTimeStyles.RoundtripKind);
            }
            catch
            {
                return DateTime.UtcNow;
            }
        }

        ///
        /// Utility method to display the a timespan element as days and minutes no matter it's size.
        /// <param name="time">The TimeSpan To be Converted</param>
        ///
        public static string ToDDHHMM(this TimeSpan time)
        {
            try
            {
                int DaysCount = (int)Math.Floor(time.TotalDays);
                string DaysText = string.Empty;
                if (DaysCount == 1) DaysText = $"{DaysCount} day ";
                if(DaysCount > 1) DaysText = $"{DaysCount} days ";
                string result = $"{DaysText}{string.Format("{0:00}h {1:00}m", time.Hours, time.Minutes)}";
                return result;
            }
            catch (Exception)
            {
                return "00:00";
            }
        }

        ///
        /// Utility method to display the a timespan element as Hours and minutes no matter it's size.
        /// <param name="time">The TimeSpan To be Converted</param>
        ///
        public static string ToHHMM(this TimeSpan time)
        {
            try
            {
                string result = string.Format("{0:00}:{1:00}", Math.Floor(time.TotalHours), time.Minutes);
                return result;
            }
            catch (Exception)
            {
                return "00:00";
            }
        }

        ///
        /// Utility method to display the a timespan element as Hours and minutes no matter it's size.
        /// <param name="time">The TimeSpan To be Converted</param>
        ///
        public static string ToHHMMSS(this TimeSpan time)
        {
            try
            {
                string result = string.Format("{0:00}:{1:00}:{2:00}", Math.Floor(time.TotalHours), time.Minutes, time.Seconds);
                return result;
            }
            catch (Exception)
            {
                return "00:00";
            }
        }

        public static string ToHHMMSS(DateTime Start, DateTime End)
        {
            try
            {
                return (End - Start).ToHHMMSS();
            }
            catch (Exception)
            {
                return "00:00:00";
            }
        }

        ///
        /// Utility method used to display DateTime in a unambiguous layout without time
        /// <param name="date">The DateTime to be processed</param>
        ///
        public static string OrderableDate(this DateTime? Date)
        {
            try
            {
                if (Date.HasValue)
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd}", Date.Value);
                }
                else return "No Date";
            }
            catch
            {
                return "No Date";
            }
        }

        /// <summary>
        /// Convert a DateTime (which might be null) from UTC timezone
        /// into the user's timezone.
        /// </summary>
        /// <param name="timezoneOffset">Offset in Minutes</param>
        /// <returns></returns>
        public static DateTime FromUTCData(this DateTime dt, int timezoneOffset)
        {
            try
            {
                return dt - new TimeSpan(timezoneOffset / 60, timezoneOffset % 60, 0);
            }
            catch (Exception)
            {
                return dt;
            }
        }

        /// <summary>
        /// Convert a DateTime (which might be null) from UTC timezone
        /// into the user's timezone.
        /// </summary>
        /// <param name="timezoneOffset">Offset in Minutes</param>
        /// <returns></returns>
        public static DateTime? FromUTCData(this DateTime? dt, int timezoneOffset)
        {
            try
            {
                if (dt.HasValue)
                {
                    return dt.Value - new TimeSpan(timezoneOffset / 60, timezoneOffset % 60, 0);
                }
                else return null;
            }
            catch (Exception)
            {
                return dt;
            }
        }

        /// <summary>
        /// Convert a TimeSpan (which might be null) from UTC timezone
        /// into the user's timezone.
        /// </summary>
        /// <param name="timezoneOffset">Offset in Minutes</param>
        /// <returns></returns>
        public static TimeSpan FromUTCData(this TimeSpan dt, int timezoneOffset)
        {
            try
            {
                TimeSpan Offset = new TimeSpan(0, timezoneOffset, 0);
                dt = dt.Subtract(Offset);
                return dt;
            }
            catch (Exception)
            {
                return dt;
            }
        }

        /// <summary>
        /// Convert a DateTime to UTC timezone
        /// from the user's timezone.
        /// </summary>
        /// <param name="timezoneOffset">Offset in Minutes</param>
        /// <returns></returns>
        public static DateTime ToUTCData(this DateTime dt, int timezoneOffset)
        {
            try
            {
                DateTime newDate = dt + new TimeSpan(timezoneOffset / 60, timezoneOffset % 60, 0);
                return newDate;
            }
            catch (Exception)
            {
                return dt;
            }
        }

        /// <summary>
        /// Convert a DateTime (which might be null) to UTC timezone
        /// from the user's timezone.
        /// </summary>
        /// <param name="timezoneOffset">Offset in Minutes</param>
        /// <returns></returns>
        public static DateTime? ToUTCData(this DateTime? dt, int timezoneOffset)
        {
            try
            {
                if (dt.HasValue)
                {
                    DateTime newDate = dt.Value + new TimeSpan(timezoneOffset / 60, timezoneOffset % 60, 0);
                    return newDate;
                }
                else return null;
            }
            catch (Exception)
            {
                return dt;
            }
        }

        /// <summary>
        /// Convert a TimeSpan to UTC timezone
        /// from the user's timezone.
        /// </summary>
        /// <param name="timezoneOffset">Offset in Minutes</param>
        /// <returns></returns>
        public static TimeSpan ToUTCData(this TimeSpan dt, int timezoneOffset)
        {
            try
            {
                TimeSpan Offset = new TimeSpan(0, timezoneOffset, 0);
                dt = dt.Subtract(Offset);
                return dt;
            }
            catch (Exception)
            {
                return dt;
            }
        }

        public static string ToISOString(this DateTime date, int TimeOffset = 0)
        {
            try
            {
                return date.FromUTCData(TimeOffset).ToString("s", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static DateTime FromISOString(this string dateString, int TimeOffset = 0)
        {
            Match match = new Regex(@"\b(\d{4})(-W(\d{2})-|W(\d{2}))(\d)(T\S+)?\b").Match(dateString);
            if (match.Success)
            {
                int year = int.Parse(match.Groups[1].Value);
                int week = int.Parse(match.Groups[3].Value + match.Groups[4].Value);
                int day = int.Parse(match.Groups[5].Value);
                if (year < 1 || year > 9999 || week < 1 || week > 53 || day < 1 || day > 7) throw new FormatException();
                DateTime firstJan = new DateTime(year, 1, 1);
                DateTime firstWeek = firstJan.DayOfWeek >= DayOfWeek.Friday
                  ? firstJan.AddDays(firstJan.DayOfWeek - DayOfWeek.Monday - 1)
                  : firstJan.AddDays(DayOfWeek.Monday - firstJan.DayOfWeek);
                DateTime fromWeekAndDay = firstWeek.AddDays(((week - 1) * 7) + day - 1);
                if (week > 51 && fromWeekAndDay > FromISOString(fromWeekAndDay.Year + "-W01-1")) throw new FormatException();
                if (match.Groups[6].Success) dateString = fromWeekAndDay.ToString("yyyy-MM-dd") + match.Groups[6].Value;
                else return fromWeekAndDay.ToUTCData(TimeOffset);
            }
            Regex excessiveFractions = new Regex(@"(\d(\.|,‎)\d{8,})");
            if (excessiveFractions.IsMatch(dateString)) dateString = excessiveFractions.Replace(dateString, m => decimal.Round(decimal.Parse(m.Value.Substring(0, Math.Max(m.Value.Length, 10)).Replace(',', '.')), 7, MidpointRounding.ToEven).ToString());
            string[] formats = new[] { "yyyy-MM-ddK", "yyyyMMddK", "yy-MM-ddK", "yyMMddK", "yyyy-MM-ddTHH:mm:ss.fffffffK", "yyyyMMddTHH:mm:ss.fffffffK", "yy-MM-ddTHH:mm:ss.fffffffK", "yyMMddTHH:mm:ss.fffffffK", "yyyy-MM-ddTHH:mm:ss,fffffffK", "yyyyMMddTHH:mm:ss,fffffffK", "yy-MM-ddTHH:mm:ss,fffffffK", "yyMMddTHH:mm:ss,fffffffK", "yyyy-MM-ddTHH:mm:ss.ffffffK", "yyyyMMddTHH:mm:ss.ffffffK", "yy-MM-ddTHH:mm:ss.ffffffK", "yyMMddTHH:mm:ss.ffffffK", "yyyy-MM-ddTHH:mm:ss,ffffffK", "yyyyMMddTHH:mm:ss,ffffffK", "yy-MM-ddTHH:mm:ss,ffffffK", "yyMMddTHH:mm:ss,ffffffK", "yyyy-MM-ddTHH:mm:ss.fffffK", "yyyyMMddTHH:mm:ss.fffffK", "yy-MM-ddTHH:mm:ss.fffffK", "yyMMddTHH:mm:ss.fffffK", "yyyy-MM-ddTHH:mm:ss,fffffK", "yyyyMMddTHH:mm:ss,fffffK", "yy-MM-ddTHH:mm:ss,fffffK", "yyMMddTHH:mm:ss,fffffK", "yyyy-MM-ddTHH:mm:ss.ffffK", "yyyyMMddTHH:mm:ss.ffffK", "yy-MM-ddTHH:mm:ss.ffffK", "yyMMddTHH:mm:ss.ffffK", "yyyy-MM-ddTHH:mm:ss,ffffK", "yyyyMMddTHH:mm:ss,ffffK", "yy-MM-ddTHH:mm:ss,ffffK", "yyMMddTHH:mm:ss,ffffK", "yyyy-MM-ddTHH:mm:ss.ffK", "yyyyMMddTHH:mm:ss.ffK", "yy-MM-ddTHH:mm:ss.ffK", "yyMMddTHH:mm:ss.ffK", "yyyy-MM-ddTHH:mm:ss,ffK", "yyyyMMddTHH:mm:ss,ffK", "yy-MM-ddTHH:mm:ss,ffK", "yyMMddTHH:mm:ss,ffK", "yyyy-MM-ddTHH:mm:ss.fK", "yyyyMMddTHH:mm:ss.fK", "yy-MM-ddTHH:mm:ss.fK", "yyMMddTHH:mm:ss.fK", "yyyy-MM-ddTHH:mm:ss,fK", "yyyyMMddTHH:mm:ss,fK", "yy-MM-ddTHH:mm:ss,fK", "yyMMddTHH:mm:ss,fK", "yyyy-MM-ddTHH:mm:ssK", "yyyyMMddTHH:mm:ssK", "yy-MM-ddTHH:mm:ssK", "yyMMddTHH:mm:ss‎K", "yyyy-MM-ddTHHmmss.fffffffK", "yyyyMMddTHHmmss.fffffffK", "yy-MM-ddTHHmmss.fffffffK", "yyMMddTHHmmss.fffffffK", "yyyy-MM-ddTHHmmss,fffffffK", "yyyyMMddTHHmmss,fffffffK", "yy-MM-ddTHHmmss,fffffffK", "yyMMddTHHmmss,fffffffK", "yyyy-MM-ddTHHmmss.ffffffK", "yyyyMMddTHHmmss.ffffffK", "yy-MM-ddTHHmmss.ffffffK", "yyMMddTHHmmss.ffffffK", "yyyy-MM-ddTHHmmss,ffffffK", "yyyyMMddTHHmmss,ffffffK", "yy-MM-ddTHHmmss,ffffffK", "yyMMddTHHmmss,ffffffK", "yyyy-MM-ddTHHmmss.fffffK", "yyyyMMddTHHmmss.fffffK", "yy-MM-ddTHHmmss.fffffK", "yyMMddTHHmmss.fffffK", "yyyy-MM-ddTHHmmss,fffffK", "yyyyMMddTHHmmss,fffffK", "yy-MM-ddTHHmmss,fffffK", "yyMMddTHHmmss,fffffK", "yyyy-MM-ddTHHmmss.ffffK", "yyyyMMddTHHmmss.ffffK", "yy-MM-ddTHHmmss.ffffK", "yyMMddTHHmmss.ffffK", "yyyy-MM-ddTHHmmss,ffffK", "yyyyMMddTHHmmss,ffffK", "yy-MM-ddTHHmmss,ffffK", "yyMMddTHHmmss,ffffK", "yyyy-MM-ddTHHmmss.ffK", "yyyyMMddTHHmmss.ffK", "yy-MM-ddTHHmmss.ffK", "yyMMddTHHmmss.ffK", "yyyy-MM-ddTHHmmss,ffK", "yyyyMMddTHHmmss,ffK", "yy-MM-ddTHHmmss,ffK", "yyMMddTHHmmss,ffK", "yyyy-MM-ddTHHmmss.fK", "yyyyMMddTHHmmss.fK", "yy-MM-ddTHHmmss.fK", "yyMMddTHHmmss.fK", "yyyy-MM-ddTHHmmss,fK", "yyyyMMddTHHmmss,fK", "yy-MM-ddTHHmmss,fK", "yyMMddTHHmmss,fK", "yyyy-MM-ddTHHmmssK", "yyyyMMddTHHmmssK", "yy-MM-ddTHHmmssK", "yyMMddTHHmmss‎K", "yyyy-MM-ddTHH:mmK", "yyyyMMddTHH:mmK", "yy-MM-ddTHH:mmK", "yyMMddTHH:mmK", "yyyy-MM-ddTHHK", "yyyyMMddTHHK", "yy-MM-ddTHHK", "yyMMddTHHK" };
            return DateTime.ParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite).ToUTCData(TimeOffset);
        }

        public static bool HasOverlaps(DateTime Date, DateTime Start, DateTime End)
        {
            return Date >= Start && Date < End;
        }

        public static bool HasOverlaps(DateTime RangeAStart, DateTime RangeAEnd, DateTime RangeBStart, DateTime RangeBEnd)
        {
            return RangeAStart < RangeBEnd && RangeBStart < RangeAEnd;
        }
    }
}