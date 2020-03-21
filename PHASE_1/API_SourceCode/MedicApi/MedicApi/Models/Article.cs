using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MedicApi.Models
{
    public class Article
    {
        public string url { get; set; }
        public List<Report> reports { get; set; }
        public string headline { get; set; }
        public string main_text { get; set; }
        public string date_of_publication { get; set; }

        public override string ToString()
        {
            return "[Article: {url: '" + url +
                "', headline: '" + headline +
                "', main_text: '" + main_text +
                "', date_of_publication: '" + date_of_publication + "'}]";
        }
    }
}
