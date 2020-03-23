using System;

namespace MedicApi.Models
{
    public class ApiResponseMetadata
    {
        public class DataSource
        {
            public readonly string name = "CDC";
            public readonly string url = "https://www.cdc.gov/outbreaks/";
        }

        public ApiResponseMetadata(DateTime accessed_time)
        {
            this.team_name = "Medics";
            this.accessed_time = accessed_time;
            this.data_source = new DataSource();
        }

        public string team_name { get; set; }
        public DateTime accessed_time { get; set; }
        public DataSource data_source { get; set; }
    }
}
