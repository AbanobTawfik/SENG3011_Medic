using System;
using System.Collections.Generic;
using System.Linq;

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

            return RetrieveReports(start, end, key_terms, location, max, offset);
        }

        public List<Article> RetrieveReports(DateTime start, DateTime end,
                                             string key_terms, string location,
                                             int max, int offset)
        {
            try
            {
                var db = client.GetDatabase("dev-reports");

                var collections = db.GetCollection<StoredArticle>("reports");

                var articles = collections.Find(x => x.url == "url1").ToList();

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
