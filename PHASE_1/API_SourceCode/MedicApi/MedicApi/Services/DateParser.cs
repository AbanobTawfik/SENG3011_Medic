using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MedicApi.Services
{
    public static class DateParser
    {
        
        public static string ParseDateStr(string s)
        {
            string res = ParseDatesRangingFromTo(s);
            if (res != null) return res;

            return "";
        }

        ////////////////////////////////////////////////////////////////////////
        // Variety of parsing methods

        // Converts a string containing a date range in the form
        // "<month name> <day>, <year> to <month name> <day>, <year>
        // into the form "yyyy-MM-dd xx:xx:xx to yyyy-MM-dd xx:xx:xx".
        public static string ParseDatesRangingFromTo(string s)
        {
            Regex re = new Regex(@"^.*(January|February|March|April|May|June|July|August|September|October|November|December)[^\d]*(\d+)[^\d]*(\d+).*?(January|February|March|April|May|June|July|August|September|October|November|December)[^\d]*(\d+)[^\d]*(\d+).*$");

            Match m = re.Match(s);
            if (m.Success)
            {
                int y1 = int.Parse(m.Groups[3].Value);      // start year
                int m1 = MonthNameToNum(m.Groups[1].Value); // start month
                int d1 = int.Parse(m.Groups[2].Value);      // start date

                int y2 = int.Parse(m.Groups[6].Value);      // end year
                int m2 = MonthNameToNum(m.Groups[4].Value); // end month
                int d2 = int.Parse(m.Groups[5].Value);      // end date

                string date = string.Format("{0}-{1:D2}-{2:D2} xx:xx:xx to {3}-{4:D2}-{5:D2} xx:xx:xx",
                                            y1, m1, d1, y2, m2, d2);
                return date;
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////
        // Helper Methods
        public static int MonthNameToNum(string monthName)
        {
            switch (monthName)
            {
                case "January":   return  1;
                case "February":  return  2;
                case "March":     return  3;
                case "April":     return  4;
                case "May":       return  5;
                case "June":      return  6;
                case "July":      return  7;
                case "August":    return  8;
                case "September": return  9;
                case "October":   return 10;
                case "November":  return 11;
                case "December":  return 12;
                default:          return  0;
            }
        }
    }
}
