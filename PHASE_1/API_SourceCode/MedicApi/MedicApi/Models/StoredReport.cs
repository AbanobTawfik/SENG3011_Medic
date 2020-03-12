﻿using System;
using System.Collections.Generic;
using System.Linq;

using MongoDB.Bson.Serialization.Attributes;

namespace MedicApi.Models
{
    [BsonIgnoreExtraElements]
    public class StoredReport
    {
        public List<string> diseases { get; set; }
        public List<string> syndromes { get; set; }
        public DateTime event_date_start { get; set; }
        public DateTime event_date_end { get; set; }
        public List<StoredPlace> locations { get; set; }
        public Report ToReport()
        {
            return new Report
            {
                diseases = diseases,
                syndromes = syndromes,
                event_date = event_date_start.ToString("yyyy-MM-dd HH:mm:ss"),
                locations = locations.Select(p => p.ToPlace()).ToList(),
            };
        }
    }
}