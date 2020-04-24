using System;
using System.Collections.Generic;

using MongoDB.Bson.Serialization.Attributes;

namespace MedicApi.Models
{
    [BsonIgnoreExtraElements]
    public class Cases
    {
        public DateTime start { get; set; }
        public int interval { get; set; }
        public List<int> data { get; set; }
    }
}
