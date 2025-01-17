﻿using MedicApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MedicApi.Services
{
    public class LocationMapper
    {
        // File paths for location geoname data
        private const string countryInfo = @"Resources\countryInfo.txt";
        private const string admin1Codes = @"Resources\admin1CodesASCII.txt";
        private const string worldCities = @"Resources\world-cities.txt";

        private Dictionary<string, int> map; // Map from location to geoname id
        private Dictionary<string, List<string>> alt; // Map from location to alternate locations
        private Dictionary<string, string[]> related;

        public LocationMapper(string baseDir)
        {
            LoadLocationMap(baseDir);
            // var x = ExtractLocations("In Sydney, Wollongong and Los Angeles");
        }

        /*
         * Returns a list of extracted locations from a given string.
         * Set the geoID flag to produce a list of GeoName IDs instead.
         * Access GeoName data using https://www.geonames.org/<GeoNameID>
         */
        public List<StoredPlace> ExtractLocations(string location)
        {
            var parts = location.Split(',').Select(p => p.Trim());
            // Try basic lookup of the specified location
            var full = String.Join(',', parts);
            if (map.ContainsKey(full))
            {   // Succeed if no alternate locations with the same name
                // and there is a perfect match
                if (alt[full].Count() == 1 || parts.Count() == 1)
                    return new List<StoredPlace> { CreatePlace(alt[full][0]) };
            }
            // Enumerate through strings formed by consecutive words
            var delimiters = new char[] { ' ', ',', '.', ':', '\n', '\t' };
            var ids = new List<StoredPlace>();
            var words = full.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++) {
                var test = "";
                for (int j = i; j < words.Length; j++) {
                    test += (test == "" ? "" : " ") + words[j];
                    if (map.ContainsKey(test))
                    {   // 'test' is a potential location
                        StoredPlace best = null;
                        var bestScore = 0;
                        // Find the best match out of the potential locations
                        foreach (var alternative in alt[test]) {
                            var altWords = alternative.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                            int k, m = 0, score = 0;
                            for (k = i; k < words.Length; k++) {
                                for (; m < altWords.Length && words[k] != altWords[m]; m++) ;
                                if (m < altWords.Length)
                                    score++;
                                else
                                    break;
                            }
                            if (score > bestScore)
                            {
                                best = CreatePlace(alternative);
                                bestScore = score;
                                j = k - 1;
                            }
                        }
                        ids.Add(best);
                        i = j;
                        break;
                    }
                }
            }
            return ids;
        }

        private void LoadLocationMap(string baseDir)
        {
            map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            alt = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            related = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            try
            {
                // Map from country code to country name (eg 'AU' to 'Australia')
                var isoMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                // Prioritise countries
                var reader = new StreamReader(Path.Combine(baseDir, countryInfo));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line[0] != '#')
                    {
                        var fields = line.Split('\t');
                        var country = fields[4];
                        // isoMap[ISO] = country
                        isoMap[fields[0]] = country;
                        related[country] = new string[] { fields[0], fields[1] };
                        // map[country] = geonameid
                        if (!map.ContainsKey(country))
                            map[country] = Int32.Parse(fields[16]);
                        if (!alt.ContainsKey(country))
                            alt[country] = new List<string>();
                        alt[country].Add(country);
                    }
                }
                reader.Close();
                // Prioritise US states and cities
                var statesPath = Path.Combine(baseDir, admin1Codes);
                var citiesPath = Path.Combine(baseDir, worldCities);
                LoadMapStates(map, alt, isoMap, statesPath, "US");
                LoadMapCities(map, alt, citiesPath, "United States");
                // Load other states and cities
                LoadMapStates(map, alt, isoMap, statesPath);
                LoadMapCities(map, alt, citiesPath);
                isoMap = null;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to generate location map: " + e.Message);
            }
        }

        private void LoadMapStates(Dictionary<string, int> map, Dictionary<string, List<string>> alt,
            Dictionary<string, string> isoMap, string statesPath, string countryCode = null)
        {
            var reader = new StreamReader(statesPath);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var fields = line.Split('\t');
                var iso = fields[0].Substring(0, 2);
                if (countryCode != null && iso != countryCode) continue;

                var state = (fields[1] != "") ? fields[1] : fields[2];
                var geoId = Int32.Parse(fields[3]);
                if (!map.ContainsKey(state))
                    map[state] = geoId;

                var full = state + "," + isoMap[iso];
                if (!alt.ContainsKey(state))
                    alt[state] = new List<string>();
                if (countryCode == null)
                    alt[state].Add(full);
                else
                    alt[state].Insert(0, full);
                map[full] = geoId;
            }
            reader.Close();
        }

        private void LoadMapCities(Dictionary<string, int> map, Dictionary<string, List<string>> alt,
            string citiesPath, string country = null)
        {
            StreamReader reader;
            // Binary search to the beginning of the specified country's cities
            if (country != null)
            {
                var length = new FileInfo(citiesPath).Length;
                var stream = File.OpenRead(citiesPath);
                long lo = 0, hi = length, mid;
                while (lo < hi)
                {
                    mid = (lo + hi) / 2;
                    stream.Seek(mid, SeekOrigin.Begin);
                    reader = new StreamReader(stream); reader.ReadLine();
                    string line = reader.ReadLine();
                    if (String.Compare(line.Split('\t')[1], country, true) < 0)
                    {
                        lo = mid + 1;
                    }
                    else
                    {
                        hi = mid;
                    }
                }
                stream.Seek(lo, SeekOrigin.Begin);
                reader = new StreamReader(stream); reader.ReadLine();
            }
            else
            {
                reader = new StreamReader(citiesPath);
            }
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var fields = line.Split('\t');
                if (country != null && fields[1] != country) break;

                var city = fields[0];
                var geoId = Int32.Parse(fields[3]);
                if (!map.ContainsKey(city))
                    map[city] = geoId;

                var full = city + "," + fields[2] + "," + fields[1];
                if (!alt.ContainsKey(city))
                    alt[city] = new List<string>();
                alt[city].Add(full);
                map[full] = geoId;
            }
            reader.Close();
        }

        private StoredPlace CreatePlace(string full)
        {
            var parts = full.Split(",");
            return new StoredPlace()
            {
                country = parts.Last(),
                location = String.Join(", ", parts.Take(parts.Length - (parts.Length > 1 ? 1 : 0))),
                geonames_id = map[full],
                location_names = parts.Union(related[parts.Last()]).ToArray()
            };
        }
    }
}
