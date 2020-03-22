using System;
using System.Collections.Generic;
using System.Text;

namespace MedicApi.Models
{
    public class ApiGetArticlesResponse
    {
        public class DataSource
        {
            public readonly string name = "CDC";
            public readonly string url = "https://www.cdc.gov/outbreaks/";
        }

        public ApiGetArticlesResponse(DateTime accessed_time,
                                      List<Article> articles)
        {
            this.team_name = "Medics";
            this.accessed_time = accessed_time;
            this.data_source = new DataSource();
            this.articles = articles;
        }

        public string team_name { get; set; }
        public DateTime accessed_time { get; set; }
        public DataSource data_source { get; set; }
        public List<Article> articles { get; set; }
    }
}
