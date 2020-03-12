﻿using System;
using System.Collections.Generic;
using System.Linq;

using MongoDB.Bson.Serialization.Attributes;

namespace MedicApi.Models
{
    [BsonIgnoreExtraElements]
    public class StoredArticle
    {
        public string url { get; set; }
        public DateTime date_of_publication { get; set; }
        public string date_of_publication_str { get; set; }
        public string headline { get; set; }
        public string main_text { get; set; }
        public List<StoredReport> reports { get; set; }

        public Article ToArticle()
        {
            return new Article
            {
                url = url,
                reports = reports.Select(r => r.ToReport()).ToList(),
                headline = headline,
                main_text = main_text,
                date_of_publication = date_of_publication_str,
            };
        }
    }
}