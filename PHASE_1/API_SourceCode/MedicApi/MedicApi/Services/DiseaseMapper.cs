using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicApi.Services
{
    public class DiseaseMapper:Mapper
    {
        public DiseaseMapper() : base()
        {
            initaliseMapper();
        }

        private void initaliseMapper()
        {
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
    }
}
