using Microsoft.VisualStudio.TestTools.UnitTesting;
using MedicApi.Models;
using MedicApi.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicApi.Services.Tests
{
    [TestClass()]
    public class LocationMapperTests
    {
        [TestMethod()]
        public void ExtractSimple()
        {
            LocationMapper service = new LocationMapper(AppContext.BaseDirectory);
            // Determine unique country
            var place = service.ExtractLocations("Australia")[0].ToPlace();
            Assert.AreEqual(place, new Place
            {
                country = "Australia",
                location = "Australia",
                geonames_id = 2077456
            });
            // Determine unique state
            place = service.ExtractLocations("New South Wales")[0].ToPlace();
            Assert.AreEqual(place, new Place
            {
                country = "Australia",
                location = "New South Wales",
                geonames_id = 2155400
            });
            // Determine unique city
            place = service.ExtractLocations("Randwick")[0].ToPlace();
            Assert.AreEqual(place, new Place
            {
                country = "Australia",
                location = "Randwick, New South Wales",
                geonames_id = 2208285
            });
        }

        [TestMethod()]
        public void ExtractStatePriority()
        {
            LocationMapper service = new LocationMapper(AppContext.BaseDirectory);
            // Correctly prioritise states over cities of the same name
            var place = service.ExtractLocations("Oregon")[0].ToPlace();
            Assert.AreEqual(place, new Place
            {
                country = "United States",
                location = "Oregon",
                geonames_id = 5744337
            });
            // Determine the correct location when given additional information
            place = service.ExtractLocations("Oregon, Ohio")[0].ToPlace();
            Assert.AreEqual(place, new Place
            {
                country = "United States",
                location = "Oregon, Ohio",
                geonames_id = 5165734
            });
            place = service.ExtractLocations("Oregon City")[0].ToPlace();
            Assert.AreEqual(place, new Place
            {
                country = "United States",
                location = "Oregon City, Oregon",
                geonames_id = 5744253
            });
        }

        [TestMethod()]
        public void ExtractInsertPriority()
        {
            LocationMapper service = new LocationMapper(AppContext.BaseDirectory);
            // Two cities named 'Sydney' but 'Sydney, NSW' was inserted earlier
            var place = service.ExtractLocations("Sydney")[0].ToPlace();
            Assert.AreEqual(place, new Place
            {
                country = "Australia",
                location = "Sydney, New South Wales",
                geonames_id = 2147714
            });
            // Determine the correct Sydney when given additional information
            place = service.ExtractLocations("Sydney, Canada")[0].ToPlace();
            Assert.AreEqual(place, new Place
            {
                country = "Canada",
                location = "Sydney, Nova Scotia",
                geonames_id = 6354908
            });
            place = service.ExtractLocations("Sydney, Nova Scotia")[0].ToPlace();
            Assert.AreEqual(place, new Place
            {
                country = "Canada",
                location = "Sydney, Nova Scotia",
                geonames_id = 6354908
            });
            // Consider locations independent if separated by another word
            place = service.ExtractLocations("Sydney and Canada")[0].ToPlace();
            Assert.AreEqual(place, new Place
            {
                country = "Australia",
                location = "Sydney, New South Wales",
                geonames_id = 2147714
            });
        }

        [TestMethod()]
        public void ExtractMultiple()
        {
            LocationMapper service = new LocationMapper(AppContext.BaseDirectory);
            // Correctly extract consecutive locations
            var places = service.ExtractLocations("Arizona, California, " +
                "Florida, Georgia, Illinois, Massachusetts, New Hampshire, " +
                "New York, Oregon, Rhode Island, Washington, and Wisconsin.");
            var expected = new Place[] {
                new Place { geonames_id = 5551752, location = "Arizona", country = "United States" },
                new Place { geonames_id = 5332921, location = "California", country = "United States" },
                new Place { geonames_id = 4155751, location = "Florida", country = "United States" },
                new Place { geonames_id = 4197000, location = "Georgia", country = "United States" },
                new Place { geonames_id = 4896861, location = "Illinois", country = "United States" },
                new Place { geonames_id = 6254926, location = "Massachusetts", country = "United States" },
                new Place { geonames_id = 5090174, location = "New Hampshire", country = "United States" },
                new Place { geonames_id = 5128638, location = "New York", country = "United States" },
                new Place { geonames_id = 5744337, location = "Oregon", country = "United States" },
                new Place { geonames_id = 5224323, location = "Rhode Island", country = "United States" },
                new Place { geonames_id = 5815135, location = "Washington", country = "United States" },
                new Place { geonames_id = 5279468, location = "Wisconsin", country = "United States" },
            };
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(places[i].ToPlace(), expected[i]);
            // Correctly identify cities in comma delimited input
            places = service.ExtractLocations("Sydney, Australia, Singapore, New Delhi, India");
            expected = new Place[] {
                new Place { geonames_id = 2147714, location = "Sydney, New South Wales", country = "Australia" },
                new Place { geonames_id = 1880251, location = "Singapore", country = "Singapore" },
                new Place { geonames_id = 1261481, location = "New Delhi, NCT", country = "India" }
            };
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(places[i].ToPlace(), expected[i]);
        }

        [TestMethod()]
        public void ExtractUnknown()
        {
            LocationMapper service = new LocationMapper(AppContext.BaseDirectory);
            // Correctly determine no known locations found
            var places = service.ExtractLocations("October");
            Assert.AreEqual(places.Count, 0);
            places = service.ExtractLocations("In October, CDC");
            Assert.AreEqual(places.Count, 0);
            places = service.ExtractLocations("Sun Hong Foods");
            Assert.AreEqual(places.Count, 0);
            places = service.ExtractLocations("Latest Outbreak Information");
            Assert.AreEqual(places.Count, 0);
            places = service.ExtractLocations("Consumers, Food Service Operators");
            Assert.AreEqual(places.Count, 0);
            places = service.ExtractLocations("Preventing Listeria");
            Assert.AreEqual(places.Count, 0);
            places = service.ExtractLocations("Reported Cases");
            Assert.AreEqual(places.Count, 0);
            places = service.ExtractLocations("");
            Assert.AreEqual(places.Count, 0);
        }
    }
}