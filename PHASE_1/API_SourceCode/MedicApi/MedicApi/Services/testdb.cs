using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicApi.Services
{
    public class testdb
    {
        public async void test()
        {
            var client = new MongoClient("mongodb+srv://medics:adfrZUBj4IF4TNibOnLxQKansolSPoW6@cluster0-nqmfu.mongodb.net/test?retryWrites=true&w=majority");
            var db = client.GetDatabase("dev-reports");
            var collections = db.GetCollection<BsonDocument>("reports");
            var document = new BsonDocument
            {
                {"name", "MongoDB" },
                {"type", "Database" },
                {"count", 1 },
                {"info", new BsonDocument
                    {
                        {"x", "poop" },
                        {"y", 102 },
                    }
                },
            };
            await collections.InsertOneAsync(document);
            
        }
    }
}
