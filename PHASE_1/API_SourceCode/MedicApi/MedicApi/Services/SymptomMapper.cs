using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicApi.Services
{
    public class SymptomMapper : Mapper
    {
        public SymptomMapper(List<string> keys) : base(keys)
        {
            // Haemorrhagic Fever
            base.AddReference("Haemorrhagic Fever", "bone ache");
            base.AddReference("Haemorrhagic Fever", "diarrhoea");
            base.AddReference("Haemorrhagic Fever", "dizziness");
            base.AddReference("Haemorrhagic Fever", "fatigue");
            base.AddReference("Haemorrhagic Fever", "fever");
            base.AddReference("Haemorrhagic Fever", "joint ache"); 
            base.AddReference("Haemorrhagic Fever", "joint pain");
            base.AddReference("Haemorrhagic Fever", "muscle ache");
            base.AddReference("Haemorrhagic Fever", "muscle pain");
            base.AddReference("Haemorrhagic Fever", "vomit");
            base.AddReference("Haemorrhagic Fever", "weakness");

            // Acute Flaccid Paralysis
            base.AddReference("Acute Flaccid Paralysis", "difficulty breathing");
            base.AddReference("Acute Flaccid Paralysis", "fever");
            base.AddReference("Acute Flaccid Paralysis", "headache");
            base.AddReference("Acute Flaccid Paralysis", "muscle weakness");
            base.AddReference("Acute Flaccid Paralysis", "nausea");
            base.AddReference("Acute Flaccid Paralysis", "pain in the arms");
            base.AddReference("Acute Flaccid Paralysis", "pain in the legs");
            base.AddReference("Acute Flaccid Paralysis", "paralysis");
            base.AddReference("Acute Flaccid Paralysis", "slurred speech");
            base.AddReference("Acute Flaccid Paralysis", "sore throat");
            base.AddReference("Acute Flaccid Paralysis", "stiff neck");
            base.AddReference("Acute Flaccid Paralysis", "tiredness");
            base.AddReference("Acute Flaccid Paralysis", "trouble breathing");
            base.AddReference("Acute Flaccid Paralysis", "vomit");

            // Acute gastroenteritis
            base.AddReference("Acute gastroenteritis", "abdominal cramps");
            base.AddReference("Acute gastroenteritis", "abdominal pain");
            base.AddReference("Acute gastroenteritis", "bloody stool");
            base.AddReference("Acute gastroenteritis", "body ache");
            base.AddReference("Acute gastroenteritis", "decreased appetite");
            base.AddReference("Acute gastroenteritis", "diarrhea");
            base.AddReference("Acute gastroenteritis", "diarrhoea");
            base.AddReference("Acute gastroenteritis", "headache");
            base.AddReference("Acute gastroenteritis", "fever");
            base.AddReference("Acute gastroenteritis", "lethargy");
            base.AddReference("Acute gastroenteritis", "loss of appetite");
            base.AddReference("Acute gastroenteritis", "muscle ache");
            base.AddReference("Acute gastroenteritis", "stomach cramps");
            base.AddReference("Acute gastroenteritis", "stomach pain");
            base.AddReference("Acute gastroenteritis", "vomit");

            // Acute respiratory syndrome
            base.AddReference("Acute respiratory syndrome", "breathing difficulties");
            base.AddReference("Acute respiratory syndrome", "chills");
            base.AddReference("Acute respiratory syndrome", "cough");
            base.AddReference("Acute respiratory syndrome", "diarrhea");
            base.AddReference("Acute respiratory syndrome", "diarrhoea");
            base.AddReference("Acute respiratory syndrome", "difficulty breathing");
            base.AddReference("Acute respiratory syndrome", "dry cough");
            base.AddReference("Acute respiratory syndrome", "fever");
            base.AddReference("Acute respiratory syndrome", "headache");
            base.AddReference("Acute respiratory syndrome", "loss of appetite");
            base.AddReference("Acute respiratory syndrome", "low blood pressure");
            base.AddReference("Acute respiratory syndrome", "lung infection"); // == pneumonia
            base.AddReference("Acute respiratory syndrome", "malaise");
            base.AddReference("Acute respiratory syndrome", "muscle ache");
            base.AddReference("Acute respiratory syndrome", "muscle pain");
            base.AddReference("Acute respiratory syndrome", "pneumonia");
            base.AddReference("Acute respiratory syndrome", "rapid breathing");
            base.AddReference("Acute respiratory syndrome", "shivering");
            base.AddReference("Acute respiratory syndrome", "shortness of breath");
            base.AddReference("Acute respiratory syndrome", "trouble breathing");

            // Influenza-like illness
            base.AddReference("Influenza-like illness", "body ache");
            base.AddReference("Influenza-like illness", "chills");
            base.AddReference("Influenza-like illness", "cough");
            base.AddReference("Influenza-like illness", "decreased appetite");
            base.AddReference("Influenza-like illness", "dry cough");
            base.AddReference("Influenza-like illness", "fatigue");
            base.AddReference("Influenza-like illness", "fever");
            base.AddReference("Influenza-like illness", "headache");
            base.AddReference("Influenza-like illness", "loss of appetite");
            base.AddReference("Influenza-like illness", "malaise");
            base.AddReference("Influenza-like illness", "muscle pain"); // plural?
            base.AddReference("Influenza-like illness", "nasal congestion");
            base.AddReference("Influenza-like illness", "nausea");
            base.AddReference("Influenza-like illness", "runny nose");
            base.AddReference("Influenza-like illness", "shivering");
            base.AddReference("Influenza-like illness", "sore throat");

            // Acute fever and rash
            base.AddReference("Acute fever and rash", "fever");
            base.AddReference("Acute fever and rash", "rash");
            base.AddReference("Acute fever and rash", "skin lesion"); // plural?

            // Fever of unknown origin
            base.AddReference("Fever of unknown Origin", "fever");

            // Encephalitis
            base.AddReference("Encephalitis", "aches in joints");
            base.AddReference("Encephalitis", "aches in muscles");
            base.AddReference("Encephalitis", "agitation");
            base.AddReference("Encephalitis", "brain inflammation");
            base.AddReference("Encephalitis", "confusion");
            base.AddReference("Encephalitis", "convulsion"); // plural?
            base.AddReference("Encephalitis", "fatigue");
            base.AddReference("Encephalitis", "fever");
            base.AddReference("Encephalitis", "hallucination");
            base.AddReference("Encephalitis", "headache"); // plural?
            base.AddReference("Encephalitis", "inflammation of the brain");
            base.AddReference("Encephalitis", "loss of consciousness");
            base.AddReference("Encephalitis", "loss of sensation");
            base.AddReference("Encephalitis", "muscle ache"); // plural?
            base.AddReference("Encephalitis", "muscle weakness");
            base.AddReference("Encephalitis", "nausea");
            base.AddReference("Encephalitis", "seizure"); // plural?
            base.AddReference("Encephalitis", "sensitivity to light");
            base.AddReference("Encephalitis", "stiff neck");
            base.AddReference("Encephalitis", "vomit");
            base.AddReference("Encephalitis", "weakness");

            // Meningitis
            base.AddReference("Meningitis", "chills");
            base.AddReference("Meningitis", "decreased appetite");
            base.AddReference("Meningitis", "drowsiness");
            base.AddReference("Meningitis", "fever");
            base.AddReference("Meningitis", "headache"); // plural?
            base.AddReference("Meningitis", "inflammation of the brain and spinal cord");
            base.AddReference("Meningitis", "inflammation of the spinal cord");
            base.AddReference("Meningitis", "irritability");
            base.AddReference("Meningitis", "lethargy");
            base.AddReference("Meningitis", "loss of appetite");
            base.AddReference("Meningitis", "nausea");
            base.AddReference("Meningitis", "seizure"); // plural?
            base.AddReference("Meningitis", "sensitivity to bright light");
            base.AddReference("Meningitis", "sensitivity to light");
            base.AddReference("Meningitis", "sleepiness");
            base.AddReference("Meningitis", "stiff neck");
            base.AddReference("Meningitis", "tiredness");
            base.AddReference("Meningitis", "vomit");
        }

        public List<string> HighestRank(List<string> symptoms)
        {
            var ret = new List<string>();
            var map = new Dictionary<string, int>();
            foreach (var key in base.GetKeys())
            {
                var count = 0;
                foreach (var symptom in symptoms)
                {
                    if (base.GetValueFromKey(key).Contains(symptom))
                    {
                        count++;
                    }
                }
                map.Add(key, count);
            }
            var max = map.Aggregate((l, r) => l.Value > r.Value ? l : r).Value;
            ret = map.Keys.Where(c => map[c] == max).ToList();
            // find all the maximums in map
            return ret;
        }
    }
}