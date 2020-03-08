using System;
using System.Collections.Generic;
using System.Linq;

namespace MedicApi.Models
{
    public class Report
    {
        public List<string> diseases { get; set; }
        public List<string> syndromes { get; set; }
        public string event_date { get; set; }
        public List<Place> locations { get; set; }
    }
}
