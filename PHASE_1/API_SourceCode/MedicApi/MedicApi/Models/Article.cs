using System;
using System.Collections.Generic;

namespace MedicApi.Models
{
    public class Article
    {
        public string Url { get; set; }
        public List<Report> Reports { get; set; }
        public string Headline { get; set; }
        public string MainText { get; set; }
        public string DateOfPublication { get; set; }
    }
}
