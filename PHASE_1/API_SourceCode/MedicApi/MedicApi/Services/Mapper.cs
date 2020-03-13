using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicApi.Services
{
    public class Mapper
    {
        public Dictionary<string, List<string>> map;
        public Mapper()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            map = new Dictionary<string, List<string>>(comparer);
        }

        public string GetCommonKeyName(string key)
        {
            var keyCheck = map.Keys.Where(s => s.ToLower() == key.ToLower()).FirstOrDefault();
            if (keyCheck == null)
            {
                foreach (var keyValue in map.Keys)
                {
                    keyCheck = map[keyValue].Where(s => s.ToLower() == keyValue.ToLower()).FirstOrDefault();
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

        public void AddKey(string key)
        {
            map.Add(key, new List<string>());
        }

    }
}
