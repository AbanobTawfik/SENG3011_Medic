using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MedicApi.Models;

namespace MedicApi.Services
{
    public class Generator
    {
        static readonly MongoClient client = new MongoClient("mongodb+srv://medics:adfrZUBj4IF4TNibOnLxQKansolSPoW6@cluster0-nqmfu.mongodb.net/test?retryWrites=true&w=majority");

        public async void GenerateAdd()
        {
            try
            {
                var db = client.GetDatabase("dev-reports");
                var collections = db.GetCollection<StoredArticle>("reports");

                List<StoredArticle> articles = new List<StoredArticle>
                {
                    new StoredArticle
                    {
                        url = "url12",
                        date_of_publication_start = new DateTime(2019, 11, 27),
                        date_of_publication_end = new DateTime(2019, 11, 27, 23, 59, 59),
                        date_of_publication_str = "2019-11-27 xx:xx:xx",
                        headline = "Headline 12",
                        main_text = "This is the main text of article 12.",
                        keywords = new List<string>
                        {
                            "coronavirus",
                            "covid-19",
                            "virus",
                        },
                        reports = new List<StoredReport>
                        {
                            new StoredReport
                            {
                                diseases = new List<string> { "COVID-19" },
                                syndromes = new List<string> { "Acute respiratory syndrome" },
                                event_date_start = new DateTime(2019, 11, 25),
                                event_date_end = new DateTime(2019, 11, 26, 23, 59, 59),
                                event_date_str = "2019-11-25 xx:xx:xx to 2019-11-26 xx:xx:xx",
                                locations = new List<StoredPlace>
                                {
                                    new StoredPlace
                                    {
                                        country = "China",
                                        location = "Wuhan",
                                    },
                                },
                            },
                        },
                    },
                };

                await collections.InsertManyAsync(articles.ToArray<StoredArticle>());
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION:\n" + e);
            }
        }
    }
}
