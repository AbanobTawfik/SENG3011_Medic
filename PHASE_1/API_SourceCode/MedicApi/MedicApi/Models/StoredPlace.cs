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

        public override bool Equals(object o)
        {
            var item = o as StoredPlace;
            if (item == null)
            {
                return false;
            }
            return item.country == this.country && item.geonames_id == this.geonames_id 
                    && item.location == this.location;
        }

        public override int GetHashCode()
        {
            // Note: *not* StringComparer; EqualityComparer<T>
            // copes with null; StringComparer doesn't.
            var comparer = EqualityComparer<string>.Default;

            // Unchecked to allow overflow, which is fine
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + comparer.GetHashCode(country);
                hash = hash * 31 + comparer.GetHashCode(geonames_id.ToString());
                hash = hash * 31 + comparer.GetHashCode(location);
                return hash;
            }
        }
    }
}
