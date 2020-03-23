using System;
using System.Collections.Generic;
using System.Linq;

using MongoDB.Bson.Serialization.Attributes;

namespace MedicApi.Models
{
    [BsonIgnoreExtraElements]
    public class StoredArticle
    {
        private string _id;
        public string url
        {
            get { return _id; }
            set { _id = value; }
        }
        public DateTime date_of_publication_start { get; set; }
        public DateTime date_of_publication_end { get; set; }
        public string date_of_publication_str { get; set; }
        public string headline { get; set; }
        public string main_text { get; set; }
        public List<string> keywords { get; set; }
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

        public override string ToString()
        {
            return "[Article: {url: '" + url +
                "', headline: '" + headline +
                "', main_text: '" + main_text +
                "', date_of_publication_str: '" + date_of_publication_str + "'}]";
        }
    }
}
