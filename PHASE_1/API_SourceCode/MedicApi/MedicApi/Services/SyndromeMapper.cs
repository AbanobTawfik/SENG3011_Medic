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
            base.AddReference("Acute gastroenteritis", "stomach flu");
            base.AddReference("Acute gastroenteritis", "diarrhoea");
            base.AddReference("Acute gastroenteritis", "diarrhea");
            base.AddReference("Acute gastroenteritis", "cramps");
            base.AddReference("Acute gastroenteritis", "vomit");
            base.AddReference("Acute gastroenteritis", "vomiting");

            // Acute respiratory syndrome
            base.AddReference("Acute respiratory syndrome", "shortness of breath");
            base.AddReference("Acute respiratory syndrome", "rapid breathing");
            base.AddReference("Acute respiratory syndrome", "low blood pressure");

            // Influenza-like illness
            base.AddReference("Influenza-like illness", "influenza");
            base.AddReference("Influenza-like illness", "shivering");
            base.AddReference("Influenza-like illness", "chills");
            base.AddReference("Influenza-like illness", "malaise");
            base.AddReference("Influenza-like illness", "dry cough");
            base.AddReference("Influenza-like illness", "loss of appetite");

            // Acute fever and rash
            base.AddReference("Acute fever and rash", "fever and rash");
            base.AddReference("Acute fever and rash", "skin lesions");
            
            // Fever of unknown origin

            // Encephalitis
            base.AddReference("Encephalitis", "inflammation of the brain");
            base.AddReference("Encephalitis", "brain inflammation");
            base.AddReference("Encephalitis", "confusion");
            base.AddReference("Encephalitis", "agitation");
            base.AddReference("Encephalitis", "hallucination");
            base.AddReference("Encephalitis", "hallucinating");
            base.AddReference("Encephalitis", "hallucinate");
            base.AddReference("Encephalitis", "loss of sensation");

            // Meningitis
            base.AddReference("Meningitis", "inflammation of the brain and spinal cord");
            base.AddReference("Meningitis", "inflammation of the spinal cord");
        }
    }
}