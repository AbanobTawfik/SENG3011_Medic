using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicApi.Models
{
    [BsonIgnoreExtraElements]
    public class FrontEndLocation
    {
        public string Country { get; set; }
        public string Location { get; set; }
        public string GeoId { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
    }
}
