using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MedicApi.Services
{
    public static class DateUtils
    {
        public static Tuple<DateTime, DateTime> DateStrToRange(string str)
        {
            Regex reExact = new Regex(@"^(?<y>\d{4})-(?<M>\d\d|xx)-(?<d>\d\d|xx) (?<H>\d\d|xx):(?<m>\d\d|xx):(?<s>\d\d|xx)$");
            Regex reRange = new Regex(@"^(?<y1>\d{4})-(?<M1>\d\d|xx)-(?<d1>\d\d|xx) (?<H1>\d\d|xx):(?<m1>\d\d|xx):(?<s1>\d\d|xx) +to +(?<y2>\d{4})-(?<M2>\d\d|xx)-(?<d2>\d\d|xx) (?<H2>\d\d|xx):(?<m2>\d\d|xx):(?<s2>\d\d|xx)$");

            Match matchExact = reExact.Match(str);
            Match matchRange = reRange.Match(str);

            if (matchExact.Success)
            {
                GroupCollection g = matchExact.Groups;
                string y = g["y"].Value;
                string M = g["M"].Value;
                string d = g["d"].Value;
                string H = g["H"].Value;
                string m = g["m"].Value;
                string s = g["s"].Value;

                int y1 = int.Parse(y);
                int y2 = int.Parse(y);

                int M1 = (M == "xx" ?  1 : int.Parse(M));
                int M2 = (M == "xx" ? 12 : int.Parse(M));

                int d1 = (d == "xx" ?                   1 : int.Parse(d));
                int d2 = (d == "xx" ? DaysInMonth(M2, y2) : int.Parse(d));

                int H1 = (H == "xx" ?  0 : int.Parse(H));
                int H2 = (H == "xx" ? 23 : int.Parse(H));

                int m1 = (m == "xx" ?  0 : int.Parse(m));
                int m2 = (m == "xx" ? 59 : int.Parse(m));

                int s1 = (s == "xx" ?  0 : int.Parse(s));
                int s2 = (s == "xx" ? 59 : int.Parse(s));

                return new Tuple<DateTime, DateTime>(
                    new DateTime(y1, M1, d1, H1, m1, s1),
                    new DateTime(y2, M2, d2, H2, m2, s2)
                );
            }
            else if (matchRange.Success)
            {
                GroupCollection g = matchRange.Groups;
                string y1s = g["y1"].Value; string y2s = g["y2"].Value;
                string M1s = g["M1"].Value; string M2s = g["M2"].Value;
                string d1s = g["d1"].Value; string d2s = g["d2"].Value;
                string H1s = g["H1"].Value; string H2s = g["H2"].Value;
                string m1s = g["m1"].Value; string m2s = g["m2"].Value;
                string s1s = g["s1"].Value; string s2s = g["s2"].Value;

                int y1 = int.Parse(y1s);
                int y2 = int.Parse(y2s);

                int M1 = (M1s == "xx" ?  1 : int.Parse(M1s));
                int M2 = (M2s == "xx" ? 12 : int.Parse(M2s));

                int d1 = (d1s == "xx" ?                   1 : int.Parse(d1s));
                int d2 = (d2s == "xx" ? DaysInMonth(M2, y2) : int.Parse(d2s));

                int H1 = (H1s == "xx" ?  0 : int.Parse(H1s));
                int H2 = (H2s == "xx" ? 23 : int.Parse(H2s));

                int m1 = (m1s == "xx" ?  0 : int.Parse(m1s));
                int m2 = (m2s == "xx" ? 59 : int.Parse(m2s));

                int s1 = (s1s == "xx" ?  0 : int.Parse(s1s));
                int s2 = (s2s == "xx" ? 59 : int.Parse(s2s));

                return new Tuple<DateTime, DateTime>(
                    new DateTime(y1, M1, d1, H1, m1, s1),
                    new DateTime(y2, M2, d2, H2, m2, s2)
                );
            }

            return null;
        }

        public static int DaysInMonth(int month, int year)
        {
            switch (month)
            {
                case  1: return 31;
                case  2: return IsLeapYear(year) ? 29 : 28;
                case  3: return 31;
                case  4: return 30;
                case  5: return 31;
                case  6: return 30;
                case  7: return 31;
                case  8: return 31;
                case  9: return 30;
                case 10: return 31;
                case 11: return 30;
                case 12: return 31;
                default: return  0;
            }
        }

        public static bool IsLeapYear(int year)
        {
            if (year % 400 == 0) {
                return true;
            } else if (year % 100 == 0) {
                return false;
            } else if (year % 4 == 0) {
                return true;
            } else {
                return false;
            }
        }
    }
}
