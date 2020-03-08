
using MongoDB.Bson.Serialization.Attributes;

namespace MedicApi.Models
{
    [BsonIgnoreExtraElements]
    public class StoredPlace
    {
        public string country { get; set; }
        public string location { get; set; }

        public Place ToPlace()
        {
            return new Place
            {
                country = country,
                location = location,
            };
        }
    }
}
