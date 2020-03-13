using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicApi.Services
{
    public class SyndromeMapper
    {
        private Dictionary<string, List<string>> map;

        public SyndromeMapper()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            map = new Dictionary<string, List<string>>(comparer);
            map.Add("Haemorrhagic Fever", new List<string>());
            map.Add("Acute Flacid Paralysis", new List<string>());
            map.Add("Acute gastroenteritis", new List<string>());
            map.Add("Acute respiratory syndrome", new List<string>());
            map.Add("Influenza-like illness", new List<string>());
            map.Add("Acute fever and rash", new List<string>());
            map.Add("Fever of unknown Origin", new List<string>());
            map.Add("Encephalitis", new List<string>());
            map.Add("Meningitis", new List<string>());
        }

        public string GetCommonSyndromeName(string syndrome)
        {
            var syndromeCheck = map.Keys.Where(s => s.ToLower() == syndrome.ToLower()).FirstOrDefault();
            if (syndromeCheck == null)
            {
                foreach (var key in map.Keys)
                {
                    syndromeCheck = map[key].Where(s => s.ToLower() == syndrome.ToLower()).FirstOrDefault();
                    if (syndromeCheck != null)
                    {
                        syndromeCheck = key;
                        break;
                    }
                }
            }
            return syndromeCheck == null ? "Other" : syndromeCheck;
        }

        public void addReference(string syndrome, string reference)
        {
            if (map.ContainsKey(syndrome))
            {
                map[syndrome].Add(reference);
            }
        }
    }

}