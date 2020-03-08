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
                        url = "url11",
                        date_of_publication = new DateTime(2020, 3, 8),
                        date_of_publication_str = "2020-03-08 xx:xx:xx",
                        headline = "Headline 11",
                        main_text = "This is the main text of article 11.",
                        reports = new List<StoredReport>
                        {
                            new StoredReport
                            {
                                diseases = new List<string> { "2019 nCov" },
                                syndromes = new List<string> { "Acute respiratory syndrome" },
                                event_date_start = new DateTime(2020, 3, 5),
                                event_date_end = new DateTime(2020, 3, 7),
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
                Console.WriteLine(e);
            }
        }
    }
}
