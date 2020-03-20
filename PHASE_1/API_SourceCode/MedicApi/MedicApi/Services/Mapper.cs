using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicApi.Services
{
    public class Mapper
    {
        private Dictionary<string, List<string>> map;
        public Mapper(List<string> keys)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            map = new Dictionary<string, List<string>>(comparer);
            initaliseMapper(keys);
        }

        private void initaliseMapper(List<string> keys)
        {
            foreach(var key in keys)
            {
                AddKey(key);
            }
        }

        public string GetCommonKeyName(string key)
        {
            var keyCheck = map.Keys.Where(s => s.ToLower() == key.ToLower()).FirstOrDefault();
            if (keyCheck == null)
            {
                foreach (var keyValue in map.Keys)
                {
                    keyCheck = map[keyValue].Where(s => s.ToLower() == key.ToLower()).FirstOrDefault();
                    if (keyCheck != null)
                    {
                        keyCheck = keyValue;
                        break;
                    }
                }
            }
            return keyCheck == null ? "Other" : keyCheck;
        }

        public void AddReference(string key, string reference)
        {
            if (map.ContainsKey(key))
            {
                map[key].Add(reference);
            }
        }

        public List<string> AllReferences()
        {
            var allReferences = new List<string>();
            foreach(var key in map.Keys)
            {
                allReferences.Add(key);
                allReferences.AddRange(map[key]);
            }
            return allReferences;
        }

        public List<string> GetKeys()
        {
            var allKeys = new List<string>();
            foreach(var key in map.Keys)
            {
                allKeys.Add(key);
            }
            return allKeys;
        }

        public List<string> GetValueFromKey(string key)
        {
            return map[key];
        }

        public void AddKey(string key)
        {
            if (!map.ContainsKey(key))
            {
                map.Add(key, new List<string>());
            }
        }

    }
}
