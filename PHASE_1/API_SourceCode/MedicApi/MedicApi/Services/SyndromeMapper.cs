using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicApi.Services
{
    public class SyndromeMapper:Mapper
    {
        public SyndromeMapper(List<string> keys) : base(keys)
        {
            // Haemorrhagic Fever
            base.AddReference("Haemorrhagic Fever", "hemorrhagic fever");

            // Acute Flaccid Paralysis
            base.AddReference("Acute Flaccid Paralysis", "paralysis");

            // Acute gastroenteritis
            base.AddReference("Acute gastroenteritis", "gastroenteritis");

            // Influenza-like illness
            base.AddReference("Influenza-like illness", "influenza");

            // Acute fever and rash
            base.AddReference("Acute fever and rash", "fever and rash");
        }
    }
}