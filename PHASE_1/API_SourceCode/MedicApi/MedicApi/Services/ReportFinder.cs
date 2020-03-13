using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

        public List<Article> FindReports(string start_date,
                                         string end_date,  string timezone,
                                         string key_terms, string location,
                                         int max, int offset)
        {
            DateTime start = DateTime.Parse(start_date); // don't use NodaTime for now
            DateTime end = DateTime.Parse(end_date);     // don't use NodaTime for now
            // ignore timezone for now, assume all timezones are AEST/AEDT

            if (location == null)
            {
                location = "";
            }

            List<string> key_term_list;
            if (key_terms == null)
            {
                key_term_list = new List<string> { };
            }
            else
            {
                key_term_list = key_terms.Split(',').ToList<string>()
                                         .Select(t => t.Trim().ToLower())
                                         .ToList();
            }

            return RetrieveReports(start, end, key_term_list, location.ToLower(), max, offset);
        }

        public List<Article> RetrieveReports(DateTime start, DateTime end,
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
    }
}
