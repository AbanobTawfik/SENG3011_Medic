using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

using MedicApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NodaTime;
using NodaTime.Text;

namespace MedicApi.Services
{
    class ReportFinder
    {
        static readonly MongoClient client = new MongoClient("mongodb+srv://medics:adfrZUBj4IF4TNibOnLxQKansolSPoW6@cluster0-nqmfu.mongodb.net/test?retryWrites=true&w=majority");

        /**********************************************************************/

        public List<Article> Retrieve(string start_date,
                                      string end_date, string timezone,
                                      string key_terms, string location,
                                      string max, string offset)
        {
            DateTime     start_date_value;
            DateTime     end_date_value;
            List<string> key_terms_value;
            string       location_value;
            int          max_value;
            int          offset_value;

            ParseRawInput(start_date, end_date, timezone, key_terms, location,
                          max, offset, out start_date_value, out end_date_value,
                          out key_terms_value, out location_value, out max_value,
                          out offset_value);

            Console.WriteLine("Parsed:");
            Console.WriteLine(start_date_value);
            Console.WriteLine(end_date_value);
            foreach (var s in key_terms_value) {
                Console.WriteLine("Key term: <" + s + ">");
            }
            Console.WriteLine("<" + location_value + ">");
            Console.WriteLine(max_value);
            Console.WriteLine(offset_value);

            return DoRetrieve(start_date_value, end_date_value, key_terms_value,
                              location_value, max_value, offset_value);
        }

        /**********************************************************************/

        public List<Article> DoRetrieve(DateTime start, DateTime end,
                                        List<string> key_terms, string location,
                                        int max, int offset)
        {
            try
            { 
                var db = client.GetDatabase("dev-reports");

                var collections = db.GetCollection<StoredArticle>("reports");

                var articles = collections.Find(a =>
                    // date match
                    (
                        a.date_of_publication_start <= end &&
                        start <= a.date_of_publication_end
                    )
                    
                    &&
                    
                    // location match
                    (
                        location == "" ||
                        a.reports.Any(r =>
                            r.locations.Any(l =>
                                l.country.ToLower().Contains(location) ||
                                l.location.ToLower().Contains(location)
                            )
                        )
                    )

                    &&

                    // key term match
                    (
                        key_terms.Count == 0 ||
                        key_terms.Any(t =>
                            a.keywords.Contains(t)
                        )
                    )
                )
                .Skip(offset)
                .Limit(max)
                .ToList();

                return articles.Select(a => a.ToArticle()).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION:\n" + e);
                return null;
            }
        }

        /**********************************************************************/
        /* Carries out type conversion, default value handling, normalisation */
        /* and reduces arguments to their maximum allowed values.             */

        private void ParseRawInput(string start_date,
                                   string end_date, string timezone,
                                   string key_terms, string location,
                                   string max, string offset,
                                   out DateTime start_date_value,
                                   out DateTime end_date_value,
                                   out List<string> key_terms_value,
                                   out string location_value,
                                   out int max_value, out int offset_value)
        {
            // clean the parameters
            CleanRawInput(start_date, end_date, timezone, key_terms, location,
                          max, offset, out start_date, out end_date, out timezone,
                          out key_terms, out location, out max, out offset);

            ParseDateRange(start_date, end_date, timezone,
                           out start_date_value, out end_date_value);

            key_terms_value = key_terms.Split(',')
                                       .Select(t => t.Trim().ToLower())
                                       .Where(t => t.Length > 0)
                                       .ToList();

            location_value  = location.ToLower();

            max_value       = max == "" ? 25 : Math.Min(int.Parse(max), 50);

            offset_value    = offset == "" ? 0 : int.Parse(offset);
        }

        private void ParseDateRange(string start_date,
                                    string end_date, string timezone,
                                    out DateTime start_date_value,
                                    out DateTime end_date_value)
        {
            TimeSpan offset = ParseTimezoneOffset(timezone);
            DateTime start_date_time = DateTime.Parse(start_date);
            DateTime end_date_time = DateTime.Parse(end_date);

            DateTimeOffset start_time_dto = new DateTimeOffset(start_date_time, offset);
            DateTimeOffset end_time_dto = new DateTimeOffset(end_date_time, offset);

            start_date_value = start_time_dto.UtcDateTime;
            end_date_value = end_time_dto.UtcDateTime;
        }

        private TimeSpan ParseTimezoneOffset(string timezone)
        {
            if (timezone == "")
            {
                timezone = "AEST";
            }

            return TimezoneUtils.ToTimeSpan(timezone);
        }

        /**********************************************************************/
        /* Checks the raw parameters passed to the API. Arguments can be null */
        /* indicating that no value was specified for the parameter.  Returns */
        /* all errors encountered.                                            */

        public ApiError CheckRawInput(string start_date,
                                      string end_date, string timezone,
                                      string key_terms, string location,
                                      string max, string offset)
        {
            ApiError e = new ApiError();

            // clean the parameters
            CleanRawInput(start_date, end_date, timezone, key_terms, location,
                          max, offset, out start_date, out end_date, out timezone,
                          out key_terms, out location, out max, out offset);

            Console.WriteLine("Cleaned:\n");
            Console.WriteLine(start_date);
            Console.WriteLine(end_date);
            Console.WriteLine(timezone);
            Console.WriteLine(key_terms);
            Console.WriteLine(location);
            Console.WriteLine(max);
            Console.WriteLine(offset);

            CheckDates(e, start_date, end_date);

            CheckTimezone(e, timezone);

            CheckMaxAndOffset(e, max, offset);

            return e;
        }

        /**********************************************************************/
        /* The CheckXYZ functions below assume string inputs are non-null     */

        private void CheckDates(ApiError e, string start_date, string end_date)
        {
            // start_date
            DateTime start_date_value = DateTime.Now;
            bool     start_date_valid = false;

            if (start_date == "")
            {
                e.AddError("start_date", "required parameter missing");
            }
            else
            {
                start_date_valid = DateTime.TryParseExact(start_date, "yyyy-MM-ddTHH:mm:ss",
                                                          CultureInfo.InvariantCulture, 0,
                                                          out start_date_value);
                if (!start_date_valid)
                {
                    e.AddError("start_date", $"invalid value '{start_date}'");
                }
            }

            // end_date
            DateTime end_date_value = DateTime.Now;
            bool     end_date_valid = false;

            if (end_date == "")
            {
                e.AddError("end_date", "required parameter missing");
            }
            else
            {
                end_date_valid = DateTime.TryParseExact(end_date, "yyyy-MM-ddTHH:mm:ss",
                                                        CultureInfo.InvariantCulture, 0,
                                                        out end_date_value);
                if (!end_date_valid)
                {
                    e.AddError("end_date", $"invalid value '{end_date}'");
                }
            }

            if (start_date_valid && end_date_valid && start_date_value > end_date_value)
            {
                e.AddError("end_date", $"end_date is before start_date");
            }
        }

        private void CheckTimezone(ApiError e, string timezone)
        {
            if (timezone != "")
            {
                if (TimezoneUtils.IsAmbiguousTimezoneAbbreviation(timezone))
                {
                    e.AddError("timezone", $"ambiguous timezone abbreviation '{timezone}' - use the timezone name instead");
                }
                else if (!TimezoneUtils.IsValidTimezoneString(timezone))
                {
                    e.AddError("timezone", $"unknown timezone '{timezone}'");
                }
            }
        }

        private void CheckMaxAndOffset(ApiError e, string max, string offset)
        {
            // max
            int  max_value = 0;
            bool max_valid;

            if (max != "")
            {
                max_valid = int.TryParse(max, out max_value);
                if (!max_valid || max_value <= 0)
                {
                    e.AddError("max", $"invalid value '{max}'");
                }
            }

            // offset
            int  offset_value = 0;
            bool offset_valid;
            
            if (offset != "")
            {
                offset_valid = int.TryParse(offset, out offset_value);
                if (!offset_valid || offset_value < 0)
                {
                    e.AddError("offset", $"invalid value '{offset}'");
                }
            }
        }

        /**********************************************************************/
        /* Trims strings and replaces null strings with the empty string.     */

        private static void CleanRawInput(string start_date,
                                          string end_date, string timezone,
                                          string key_terms, string location,
                                          string max, string offset,
                                          out string start_date_c,
                                          out string end_date_c, out string timezone_c,
                                          out string key_terms_c, out string location_c,
                                          out string max_c, out string offset_c)
        {
            start_date_c = CleanString(start_date);
            end_date_c = CleanString(end_date);
            timezone_c = CleanString(timezone);
            if (key_terms == null)
            {
                key_terms_c = "";
            }
            else
            {
                key_terms_c = string.Join(',', key_terms.Split(',').Select(t => t.Trim()));
            }
            location_c = CleanString(location);
            max_c = CleanString(max);
            offset_c = CleanString(offset);
        }

        private static string CleanString(string s)
        {
            if (s == null)
            {
                return "";
            }
            else
            {
                return s.Trim();
            }
        }
    }
}
