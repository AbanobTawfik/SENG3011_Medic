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
            base.AddReference("Haemorrhagic Fever", "fever");
            base.AddReference("Haemorrhagic Fever", "fatigue");
            base.AddReference("Haemorrhagic Fever", "dizziness");
            base.AddReference("Haemorrhagic Fever", "muscle ache");
            base.AddReference("Haemorrhagic Fever", "muscle aches");
            base.AddReference("Haemorrhagic Fever", "bone ache");
            base.AddReference("Haemorrhagic Fever", "bone aches");
            base.AddReference("Haemorrhagic Fever", "joint ache");
            base.AddReference("Haemorrhagic Fever", "joint aches");
            base.AddReference("Haemorrhagic Fever", "weakness");


            // Acute Flaccid Paralysis
            base.AddReference("Acute Flaccid Paralysis", "sore throat");
            base.AddReference("Acute Flaccid Paralysis", "tiredness");
            base.AddReference("Acute Flaccid Paralysis", "fever");
            base.AddReference("Acute Flaccid Paralysis", "headache");
            base.AddReference("Acute Flaccid Paralysis", "head ache");
            base.AddReference("Acute Flaccid Paralysis", "slurred speech");
            base.AddReference("Acute Flaccid Paralysis", "trouble breathing");
            base.AddReference("Acute Flaccid Paralysis", "Nausea");
            base.AddReference("Acute Flaccid Paralysis", "vomiting");
            base.AddReference("Acute Flaccid Paralysis", "stiff neck");
            base.AddReference("Acute Flaccid Paralysis", "pain in the arms");
            base.AddReference("Acute Flaccid Paralysis", "pain in the legs");

            // Acute gastroenteritis
            base.AddReference("Acute gastroenteritis", "fever");
            base.AddReference("Acute gastroenteritis", "abdominal cramps");
            base.AddReference("Acute gastroenteritis", "abdominal pain");
            base.AddReference("Acute gastroenteritis", "stomach pain");
            base.AddReference("Acute gastroenteritis", "stomach cramps");
            base.AddReference("Acute gastroenteritis", "vomiting");
            base.AddReference("Acute gastroenteritis", "vomit");
            base.AddReference("Acute gastroenteritis", "muscle ache");
            base.AddReference("Acute gastroenteritis", "muscle aches");
            base.AddReference("Acute gastroenteritis", "headache");
            base.AddReference("Acute gastroenteritis", "head ache");

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
            base.AddReference("Acute fever and rash", "fever");
            base.AddReference("Acute fever and rash", "rash");
            base.AddReference("Acute fever and rash", "skin lesions");

            // Fever of unknown origin
            base.AddReference("Fever of unknown Origin", "fever");

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

        public List<string> HighestRank(List<string> symptoms)
        {
            var ret = new List<string>();
            var max = -1;
            var currentSymptom = "NOTHING";
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
                if (count > max && count > 0)
                {
                    max = count;
                    currentSymptom = key;
                }
                if (count == max)
                {
                    ret.Add(currentSymptom);
                    currentSymptom = key;
                }
            }
            if (!ret.Contains(currentSymptom))
            {
                ret.Add(currentSymptom);
            }
            return currentSymptom == "NOTHING" ? new List<string>() : ret;
        }
    }
}