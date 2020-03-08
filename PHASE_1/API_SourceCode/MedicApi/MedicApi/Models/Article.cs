using System;
using System.Collections.Generic;
using System.Linq;

namespace MedicApi.Models
{
    public class Article
    {
        public string url { get; set; }
        public List<Report> reports { get; set; }
        public string headline { get; set; }
        public string main_text { get; set; }
        public string date_of_publication { get; set; }
    }
}
