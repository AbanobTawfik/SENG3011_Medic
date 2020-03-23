using System;
using System.Collections.Generic;
using System.Text;

namespace MedicApi.Models
{
    public class Place
    {
        public string country { get; set; }
        public string location { get; set; }
        public int geonames_id { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Place other)) return false;
            return (this.country == other.country
                && this.location == other.location
                && this.geonames_id == other.geonames_id);
        }
    }
}
