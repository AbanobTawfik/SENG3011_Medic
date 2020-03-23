using System.Collections.Generic;

using MongoDB.Bson.Serialization.Attributes;

namespace MedicApi.Models
{
    [BsonIgnoreExtraElements]
    public class StoredPlace
    {
        public string country { get; set; }
        public string location { get; set; }
        public int geonames_id { get; set; }
        public string[] location_names { get; set; }

        public Place ToPlace()
        {
            return new Place
            {
                country = country,
                location = location,
                geonames_id = geonames_id,
            };
        }
    }
}
