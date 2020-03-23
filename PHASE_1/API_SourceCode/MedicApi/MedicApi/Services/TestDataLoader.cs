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
            try
            {
                List<NewStoredArticle> articles = JsonConvert.DeserializeObject<List<NewStoredArticle>>(File.ReadAllText(@"TestScripts\Data\data-1.json"));

                var db = client.GetDatabase("test-articles");
                var collections = db.GetCollection<NewStoredArticle>("articles");
                await collections.InsertManyAsync(articles);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:\n" + e.Message);
            }
        }
    }
}
