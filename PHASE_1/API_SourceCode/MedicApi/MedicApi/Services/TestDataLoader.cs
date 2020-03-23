using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

using MedicApi.Models;


namespace MedicApi.Services
{
    public static class TestDataLoader
    {
        static readonly MongoClient client = new MongoClient("mongodb+srv://medics:adfrZUBj4IF4TNibOnLxQKansolSPoW6@cluster0-nqmfu.mongodb.net/test?retryWrites=true&w=majority");

        public static async void LoadTestData()
        {
            List<string> testDataFilenames = new List<string>
            {
                @"TestData\data1.json",
                @"TestData\data2.json",
                @"TestData\data3.json",
                @"TestData\data4.json",
                @"TestData\data5.json",
                @"TestData\data6.json",
            };

            try
            {
                List<StoredArticle> articles = new List<StoredArticle>();
                foreach (string filename in testDataFilenames)
                {
                    articles.AddRange(JsonConvert.DeserializeObject<List<StoredArticle>>(File.ReadAllText(filename)));
                }

                var db = client.GetDatabase("test-articles");
                var collections = db.GetCollection<StoredArticle>("articles");
                await collections.InsertManyAsync(articles);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:\n" + e.Message);
            }
        }
    }
}
