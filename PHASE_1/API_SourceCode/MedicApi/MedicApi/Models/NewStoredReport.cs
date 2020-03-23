using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MedicApi.Models
{
    class NewStoredReport
    {
        public List<string> diseases { get; set; }
        public List<string> syndromes { get; set; }
        public DateTime event_date_start { get; set; }
        public DateTime event_date_end { get; set; }
        public string event_date_str { get; set; }
        public List<NewStoredLocation> locations { get; set; }
        public Report ToReport()
        {
            return new Report
            {
                diseases = diseases,
                syndromes = syndromes,
                event_date = event_date_str,
                locations = locations.Select(p => p.ToPlace()).ToList(),
            };
        }
    }
}
