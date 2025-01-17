﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// NOTES:
// - All public methods should normalise given timezone strings
// - All private methods assume that given strings are normalised

namespace MedicApi.Services
{
    public static class TimezoneUtils
    {
        /**********************************************************************/

        public static TimeSpan ToTimeSpan(string tz)
        {
            tz = Normalize(tz);

            TimeSpan res;
            if (TryToTimeSpan(tz, out res))
            {
                return res;
            }
            else
            {
                throw new FormatException($"invalid timezone: '{tz}'");
            }
        }

        /**********************************************************************/

        public static bool TryToTimeSpan(string tz, out TimeSpan ts)
        {
            tz = Normalize(tz);

            if (!IsValidTimezoneString(tz))
            {
                ts = new TimeSpan();
                return false;
            }

            string offset = ToOffsetString(tz);
            ts = ParseOffsetString(offset);
            return true;
        }

        // Assumes timezone string is valid
        private static string ToOffsetString(string tz)
        {
            if (tzAbbreviations.ContainsKey(tz))
            {
                return tzAbbreviations[tz];
            }
            else
            {
                return tzNames[tz];
            }
        }

        private static TimeSpan ParseOffsetString(string offset)
        {
            Regex re = new Regex(@"([+−±])([0-9]{2}):([0-9]{2})");

            Match m = re.Match(offset);

            string sign = m.Groups[1].Value;
            int hours = int.Parse(m.Groups[2].Value);
            int minutes = int.Parse(m.Groups[3].Value);
            if (sign == "−")
            {
                hours *= -1;
                minutes *= -1;
            }

            TimeSpan ts = new TimeSpan(hours, minutes, 0);
            return ts;
        }

        /**********************************************************************/

        public static bool IsAmbiguousTimezoneAbbreviation(string tz)
        {
            tz = Normalize(tz);

            return ambiguousTzAbbreviations.Contains(tz);
        }

        public static bool IsValidTimezoneString(string tz)
        {
            tz = Normalize(tz);

            return tzAbbreviations.ContainsKey(tz) || tzNames.ContainsKey(tz);
        }

        /**********************************************************************/

        private static string Normalize(string tz)
        {
            return tz.Trim().ToLower()
                     .Replace("-", " ")
                     .Replace("–", " ")
                     .Replace("–", " ")
                     .Replace("á", "a")
                     .Replace("é", "e")
                     .Replace("í", "i")
            ;
        }

        /**********************************************************************/

        // normalised timezone names
        private static Dictionary<string, string> tzNames = new Dictionary<string, string>
        {
            {"australian central daylight savings time",           "+10:30"},
            {"australian central standard time",                   "+09:30"},
            {"acre time",                                          "−05:00"},
            {"australian central western standard time",           "+08:45"},
            {"atlantic daylight time",                             "−03:00"},
            {"australian eastern daylight savings time",           "+11:00"},
            {"australian eastern standard time",                   "+10:00"},
            {"afghanistan time",                                   "+04:30"},
            {"alaska daylight time",                               "−08:00"},
            {"alaska standard time",                               "−09:00"},
            {"alma ata time",                                      "+06:00"},
            {"amazon summer time",                                 "−03:00"},
            {"amazon time",                                        "−04:00"},
            {"armenia time",                                       "+04:00"},
            {"anadyr time",                                        "+12:00"},
            {"aqtobe time",                                        "+05:00"},
            {"argentina time",                                     "−03:00"},
            {"arabia standard time",                               "+03:00"},
            {"atlantic standard time",                             "−04:00"},
            {"australian western standard time",                   "+08:00"},
            {"azores summer time",                                 "±00:00"},
            {"azores standard time",                               "−01:00"},
            {"azerbaijan time",                                    "+04:00"},
            {"brunei time",                                        "+08:00"},
            {"british indian ocean time",                          "+06:00"},
            {"baker island time",                                  "−12:00"},
            {"bolivia time",                                       "−04:00"},
            {"brasilia summer time",                               "−02:00"},
            {"brasilia time",                                      "−03:00"},
            {"bangladesh standard time",                           "+06:00"},
            {"bougainville standard time",                         "+11:00"},
            {"bhutan time",                                        "+06:00"},
            {"central africa time",                                "+02:00"},
            {"cocos islands time",                                 "+06:30"},
            {"central daylight time",                              "−05:00"},
            {"cuba daylight time",                                 "−04:00"},
            {"central european summer time",                       "+02:00"},
            {"central european time",                              "+01:00"},
            {"chatham daylight time",                              "+13:45"},
            {"chatham standard time",                              "+12:45"},
            {"choibalsan standard time",                           "+08:00"},
            {"choibalsan summer time",                             "+09:00"},
            {"chamorro standard time",                             "+10:00"},
            {"chuuk time",                                         "+10:00"},
            {"clipperton island standard time",                    "−08:00"},
            {"central indonesia time",                             "+08:00"},
            {"cook island time",                                   "−10:00"},
            {"chile summer time",                                  "−03:00"},
            {"chile standard time",                                "−04:00"},
            {"colombia summer time",                               "−04:00"},
            {"colombia time",                                      "−05:00"},
            {"central standard time",                              "−06:00"},
            {"china standard time",                                "+08:00"},
            {"cuba standard time",                                 "−05:00"},
            {"china time",                                         "+08:00"},
            {"cape verde time",                                    "−01:00"},
            {"central western standard time",                      "+08:45"},
            {"christmas island time",                              "+07:00"},
            {"davis time",                                         "+07:00"},
            {"dumont d'urville time",                              "+10:00"},
            {"easter island summer time",                          "−05:00"},
            {"easter island standard time",                        "−06:00"},
            {"east africa time",                                   "+03:00"},
            {"eastern caribbean time",                             "−04:00"},
            {"ecuador time",                                       "−05:00"},
            {"eastern daylight time",                              "−04:00"},
            {"eastern european summer time",                       "+03:00"},
            {"eastern european time",                              "+02:00"},
            {"eastern greenland summer time",                      "±00:00"},
            {"eastern greenland time",                             "−01:00"},
            {"eastern indonesian time",                            "+09:00"},
            {"eastern standard time",                              "−05:00"},
            {"further eastern european time",                      "+03:00"},
            {"fiji time",                                          "+12:00"},
            {"falkland islands summer time",                       "−03:00"},
            {"falkland islands time",                              "−04:00"},
            {"fernando de noronha time",                           "−02:00"},
            {"galapagos time",                                     "−06:00"},
            {"gambier islands time",                               "−09:00"},
            {"georgia standard time",                              "+04:00"},
            {"french guiana time",                                 "−03:00"},
            {"gilbert island time",                                "+12:00"},
            {"gambier island time",                                "−09:00"},
            {"greenwich mean time",                                "±00:00"},
            {"south georgia and the south sandwich islands time",  "−02:00"},
            {"gulf standard time",                                 "+04:00"},
            {"guyana time",                                        "−04:00"},
            {"hawaii aleutian daylight time",                      "−09:00"},
            {"heure avancee d'europe centrale",                    "+02:00"},
            {"hawaii aleutian standard time",                      "−10:00"},
            {"hong kong time",                                     "+08:00"},
            {"heard and mcdonald islands time",                    "+05:00"},
            {"hovd summer time",                                   "+08:00"},
            {"hovd time",                                          "+07:00"},
            {"indochina time",                                     "+07:00"},
            {"international day line west time zone",              "−12:00"},
            {"israel daylight time",                               "+03:00"},
            {"indian ocean time",                                  "+03:00"},
            {"iran daylight time",                                 "+04:30"},
            {"irkutsk time",                                       "+08:00"},
            {"iran standard time",                                 "+03:30"},
            {"indian standard time",                               "+05:30"},
            {"irish standard time",                                "+01:00"},
            {"israel standard time",                               "+02:00"},
            {"japan standard time",                                "+09:00"},
            {"kaliningrad time",                                   "+02:00"},
            {"kyrgyzstan time",                                    "+06:00"},
            {"kosrae time",                                        "+11:00"},
            {"krasnoyarsk time",                                   "+07:00"},
            {"korea standard time",                                "+09:00"},
            {"lord howe standard time",                            "+10:30"},
            {"lord howe summer time",                              "+11:00"},
            {"line islands time",                                  "+14:00"},
            {"magadan time",                                       "+12:00"},
            {"marquesas islands time",                             "−09:30"},
            {"mawson station time",                                "+05:00"},
            {"mountain daylight time",                             "−06:00"},
            {"middle european time",                               "+01:00"},
            {"middle european summer time",                        "+02:00"},
            {"marshall islands time",                              "+12:00"},
            {"macquarie island station time",                      "+11:00"},
            {"myanmar standard time",                              "+06:30"},
            {"moscow time",                                        "+03:00"},
            {"malaysia standard time",                             "+08:00"},
            {"mountain standard time",                             "−07:00"},
            {"mauritius time",                                     "+04:00"},
            {"maldives time",                                      "+05:00"},
            {"malaysia time",                                      "+08:00"},
            {"new caledonia time",                                 "+11:00"},
            {"newfoundland daylight time",                         "−02:30"},
            {"norfolk island time",                                "+11:00"},
            {"novosibirsk time",                                   "+07:00"},
            {"nepal time",                                         "+05:45"},
            {"newfoundland standard time",                         "−03:30"},
            {"newfoundland time",                                  "−03:30"},
            {"niue time",                                          "−11:00"},
            {"new zealand daylight time",                          "+13:00"},
            {"new zealand standard time",                          "+12:00"},
            {"omsk time",                                          "+06:00"},
            {"oral time",                                          "+05:00"},
            {"pacific daylight time",                              "−07:00"},
            {"peru time",                                          "−05:00"},
            {"kamchatka time",                                     "+12:00"},
            {"papua new guinea time",                              "+10:00"},
            {"phoenix island time",                                "+13:00"},
            {"philippine time",                                    "+08:00"},
            {"pakistan standard time",                             "+05:00"},
            {"saint pierre and miquelon daylight time",            "−02:00"},
            {"saint pierre and miquelon standard time",            "−03:00"},
            {"pohnpei standard time",                              "+11:00"},
            {"pacific standard time",                              "−08:00"},
            {"philippine standard time",                           "+08:00"},
            {"paraguay summer time",                               "−03:00"},
            {"paraguay time",                                      "−04:00"},
            {"reunion time",                                       "+04:00"},
            {"rothera research station time",                      "−03:00"},
            {"sakhalin island time",                               "+11:00"},
            {"samara time",                                        "+04:00"},
            {"south african standard time",                        "+02:00"},
            {"solomon islands time",                               "+11:00"},
            {"seychelles time",                                    "+04:00"},
            {"samoa daylight time",                                "−10:00"},
            {"singapore time",                                     "+08:00"},
            {"sri lanka standard time",                            "+05:30"},
            {"srednekolymsk time",                                 "+11:00"},
            {"suriname time",                                      "−03:00"},
            {"samoa standard time",                                "−11:00"},
            {"singapore standard time",                            "+08:00"},
            {"showa station time",                                 "+03:00"},
            {"tahiti time",                                        "−10:00"},
            {"thailand standard time",                             "+07:00"},
            {"french southern and antarctic time",                 "+05:00"},
            {"tajikistan time",                                    "+05:00"},
            {"tokelau time",                                       "+13:00"},
            {"timor leste time",                                   "+09:00"},
            {"turkmenistan time",                                  "+05:00"},
            {"turkey time",                                        "+03:00"},
            {"tonga time",                                         "+13:00"},
            {"tuvalu time",                                        "+12:00"},
            {"ulaanbaatar summer time",                            "+09:00"},
            {"ulaanbaatar standard time",                          "+08:00"},
            {"coordinated universal time",                         "±00:00"},
            {"uruguay summer time",                                "−02:00"},
            {"uruguay standard time",                              "−03:00"},
            {"uzbekistan time",                                    "+05:00"},
            {"venezuelan standard time",                           "−04:00"},
            {"vladivostok time",                                   "+10:00"},
            {"volgograd time",                                     "+04:00"},
            {"vostok station time",                                "+06:00"},
            {"vanuatu time",                                       "+11:00"},
            {"wake island time",                                   "+12:00"},
            {"west africa summer time",                            "+02:00"},
            {"west africa time",                                   "+01:00"},
            {"western european summer time",                       "+01:00"},
            {"western european time",                              "±00:00"},
            {"western indonesian time",                            "+07:00"},
            {"west greenland summer time",                         "−02:00"},
            {"west greenland time",                                "−03:00"},
            {"western standard time",                              "+08:00"},
            {"yakutsk time",                                       "+09:00"},
            {"yekaterinburg time",                                 "+05:00"},
        };

        private static HashSet<string> ambiguousTzAbbreviations = new HashSet<string>
        {
            "amt", "ast", "bst", "cdt", "cst", "ect", "gst", "ist", "lhst", "pst", "sst",
        };

        private static Dictionary<string, string> tzAbbreviations = new Dictionary<string, string>
        {
            { "acdt",  "+10:30" },
            { "acst",  "+09:30" },
            { "act",   "−05:00" },
            { "acwst", "+08:45" },
            { "adt",   "−03:00" },
            { "aedt",  "+11:00" },
            { "aest",  "+10:00" },
            { "aft",   "+04:30" },
            { "akdt",  "−08:00" },
            { "akst",  "−09:00" },
            { "almt",  "+06:00" },
            { "amst",  "−03:00" },
            { "anat",  "+12:00" },
            { "aqtt",  "+05:00" },
            { "art",   "−03:00" },
            { "awst",  "+08:00" },
            { "azost", "±00:00" },
            { "azot",  "−01:00" },
            { "azt",   "+04:00" },
            { "bdt",   "+08:00" },
            { "biot",  "+06:00" },
            { "bit",   "−12:00" },
            { "bot",   "−04:00" },
            { "brst",  "−02:00" },
            { "brt",   "−03:00" },
            { "btt",   "+06:00" },
            { "cat",   "+02:00" },
            { "cct",   "+06:30" },
            { "cest",  "+02:00" },
            { "cet",   "+01:00" },
            { "chadt", "+13:45" },
            { "chast", "+12:45" },
            { "chot",  "+08:00" },
            { "chost", "+09:00" },
            { "chst",  "+10:00" },
            { "chut",  "+10:00" },
            { "cist",  "−08:00" },
            { "cit",   "+08:00" },
            { "ckt",   "−10:00" },
            { "clst",  "−03:00" },
            { "clt",   "−04:00" },
            { "cost",  "−04:00" },
            { "cot",   "−05:00" },
            { "ct",    "+08:00" },
            { "cvt",   "−01:00" },
            { "cwst",  "+08:45" },
            { "cxt",   "+07:00" },
            { "davt",  "+07:00" },
            { "ddut",  "+10:00" },
            { "dft",   "+01:00" },
            { "easst", "−05:00" },
            { "east",  "−06:00" },
            { "eat",   "+03:00" },
            { "edt",   "−04:00" },
            { "eest",  "+03:00" },
            { "eet",   "+02:00" },
            { "egst",  "±00:00" },
            { "egt",   "−01:00" },
            { "eit",   "+09:00" },
            { "est",   "−05:00" },
            { "fet",   "+03:00" },
            { "fjt",   "+12:00" },
            { "fkst",  "−03:00" },
            { "fkt",   "−04:00" },
            { "fnt",   "−02:00" },
            { "galt",  "−06:00" },
            { "gamt",  "−09:00" },
            { "get",   "+04:00" },
            { "gft",   "−03:00" },
            { "gilt",  "+12:00" },
            { "git",   "−09:00" },
            { "gmt",   "±00:00" },
            { "gyt",   "−04:00" },
            { "hdt",   "−09:00" },
            { "haec",  "+02:00" },
            { "hst",   "−10:00" },
            { "hkt",   "+08:00" },
            { "hmt",   "+05:00" },
            { "hovst", "+08:00" },
            { "hovt",  "+07:00" },
            { "ict",   "+07:00" },
            { "idlw",  "−12:00" },
            { "idt",   "+03:00" },
            { "iot",   "+03:00" },
            { "irdt",  "+04:30" },
            { "irkt",  "+08:00" },
            { "irst",  "+03:30" },
            { "jst",   "+09:00" },
            { "kalt",  "+02:00" },
            { "kgt",   "+06:00" },
            { "kost",  "+11:00" },
            { "krat",  "+07:00" },
            { "kst",   "+09:00" },
            { "lint",  "+14:00" },
            { "magt",  "+12:00" },
            { "mart",  "−09:30" },
            { "mawt",  "+05:00" },
            { "mdt",   "−06:00" },
            { "met",   "+01:00" },
            { "mest",  "+02:00" },
            { "mht",   "+12:00" },
            { "mist",  "+11:00" },
            { "mit",   "−09:30" },
            { "mmt",   "+06:30" },
            { "msk",   "+03:00" },
            { "mut",   "+04:00" },
            { "mvt",   "+05:00" },
            { "myt",   "+08:00" },
            { "nct",   "+11:00" },
            { "ndt",   "−02:30" },
            { "nft",   "+11:00" },
            { "novt",  "+07:00" },
            { "npt",   "+05:45" },
            { "nst",   "−03:30" },
            { "nt",    "−03:30" },
            { "nut",   "−11:00" },
            { "nzdt",  "+13:00" },
            { "nzst",  "+12:00" },
            { "omst",  "+06:00" },
            { "orat",  "+05:00" },
            { "pdt",   "−07:00" },
            { "pet",   "−05:00" },
            { "pett",  "+12:00" },
            { "pgt",   "+10:00" },
            { "phot",  "+13:00" },
            { "pht",   "+08:00" },
            { "pkt",   "+05:00" },
            { "pmdt",  "−02:00" },
            { "pmst",  "−03:00" },
            { "pont",  "+11:00" },
            { "pyst",  "−03:00" },
            { "pyt",   "−04:00" },
            { "ret",   "+04:00" },
            { "rott",  "−03:00" },
            { "sakt",  "+11:00" },
            { "samt",  "+04:00" },
            { "sast",  "+02:00" },
            { "sbt",   "+11:00" },
            { "sct",   "+04:00" },
            { "sdt",   "−10:00" },
            { "sgt",   "+08:00" },
            { "slst",  "+05:30" },
            { "sret",  "+11:00" },
            { "srt",   "−03:00" },
            { "syot",  "+03:00" },
            { "taht",  "−10:00" },
            { "tha",   "+07:00" },
            { "tft",   "+05:00" },
            { "tjt",   "+05:00" },
            { "tkt",   "+13:00" },
            { "tlt",   "+09:00" },
            { "tmt",   "+05:00" },
            { "trt",   "+03:00" },
            { "tot",   "+13:00" },
            { "tvt",   "+12:00" },
            { "ulast", "+09:00" },
            { "ulat",  "+08:00" },
            { "utc",   "±00:00" },
            { "uyst",  "−02:00" },
            { "uyt",   "−03:00" },
            { "uzt",   "+05:00" },
            { "vet",   "−04:00" },
            { "vlat",  "+10:00" },
            { "volt",  "+04:00" },
            { "vost",  "+06:00" },
            { "vut",   "+11:00" },
            { "wakt",  "+12:00" },
            { "wast",  "+02:00" },
            { "wat",   "+01:00" },
            { "west",  "+01:00" },
            { "wet",   "±00:00" },
            { "wit",   "+07:00" },
            { "wgst",  "−02:00" },
            { "wgt",   "−03:00" },
            { "wst",   "+08:00" },
            { "yakt",  "+09:00" },
            { "yekt",  "+05:00" },

            // ambiguous abbreviations
            // { "amt",   "−04:00" },
            // { "amt",   "+04:00" },
            // { "ast",   "+03:00" },
            // { "ast",   "−04:00" },
            // { "bst",   "+06:00" },
            // { "bst",   "+11:00" },
            // { "cdt",   "−05:00" },
            // { "cdt",   "−04:00" },
            // { "cst",   "−06:00" },
            // { "cst",   "+08:00" },
            // { "cst",   "−05:00" },
            // { "ect",   "−04:00" },
            // { "ect",   "−05:00" },
            // { "gst",   "−02:00" },
            // { "gst",   "+04:00" },
            // { "ist",   "+05:30" },
            // { "ist",   "+01:00" },
            // { "ist",   "+02:00" },
            // { "lhst",  "+10:30" },
            // { "lhst",  "+11:00" },
            // { "mst",   "+08:00" },
            // { "mst",   "−07:00" },
            // { "pst",   "−08:00" },
            // { "pst",   "+08:00" },
            // { "sst",   "−11:00" },
            // { "sst",   "+08:00" },
        };
    }
}
