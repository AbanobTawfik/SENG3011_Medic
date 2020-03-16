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

            return DoRetrieve(start_date_value, end_date_value, key_terms_value,
                              location_value.ToLower(), max_value, offset_value);
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

        /// <summary>
        /// Converts the raw parameters into their proper types. Handles default
        /// values and reduces parameters to their maximum allowed values.
        /// </summary>
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
            start_date_value = DateTime.Parse(start_date);
            end_date_value   = DateTime.Parse(end_date);

            if (key_terms == null)
            {
                key_terms_value = new List<string> { };
            }
            else
            {
                key_terms_value = key_terms.Split(',').ToList<string>()
                                           .Select(t => t.Trim().ToLower())
                                           .ToList();
            }

            location_value   = location == null ? "" : location;

            max_value        = max == null ? 25 : Math.Min(int.Parse(max), 50);

            offset_value     = offset == null ? 0 : int.Parse(offset);
        }

        /**********************************************************************/

        /// <summary>
        /// Checks the raw parameters passed to the API.  Arguments can be null,
        /// indicating that no value was specified for the parameter.
        /// </summary>
        /// <returns>All errors encountered</returns>
        public ApiError CheckRawInput(string start_date,
                                      string end_date, string timezone,
                                      string key_terms, string location,
                                      string max, string offset)
        {
            ApiError e = new ApiError();

            CheckDates(e, start_date, end_date);

            CheckTimezone(e, timezone);

            CheckMaxAndOffset(e, max, offset);

            return e;
        }

        private void CheckDates(ApiError e, string start_date, string end_date)
        {
            // start_date
            DateTime start_date_value = DateTime.Now;
            bool     start_date_valid = false;

            if (start_date == null)
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

            if (end_date == null)
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
            if (timezone != null)
            {
                if (TimezoneUtils.IsDuplicateAbbreviation(timezone))
                {
                    e.AddError("timezone", $"ambiguous timezone abbreviation '{timezone}'; use the timezone name instead");
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

            if (max != null)
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
            
            if (offset != null)
            {
                offset_valid = int.TryParse(offset, out offset_value);
                if (!offset_valid || offset_value < 0)
                {
                    e.AddError("offset", $"invalid value '{offset}'");
                }
            }
        }
    }
}
