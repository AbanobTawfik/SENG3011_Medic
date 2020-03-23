using System;
using System.Collections.Generic;
using System.Text;

namespace MedicApi.Models
{
    class NewStoredLocation
    {
        public string country { get; set; }
        public string location { get; set; }
        public string geonames_id { get; set; }
        public List<string> location_names { get; set; }

        public Place ToPlace()
        {
            return new Place
            {
                country = country,
                location = location,
            };
        }
    }
}
