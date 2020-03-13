using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicApi.Services
{
    public class SyndromeMapper:Mapper
    {
        public SyndromeMapper():base()
        {
            initaliseMapper();
        }

        private void initaliseMapper()
        {
            AddKey("Haemorrhagic Fever");
            AddKey("Acute Flacid Paralysis");
            AddKey("Acute gastroenteritis");
            AddKey("Acute respiratory syndrome");
            AddKey("Influenza-like illness");
            AddKey("Acute fever and rash");
            AddKey("Fever of unknown Origin");
            AddKey("Encephalitis");
            AddKey("Meningitis");
        }
    }

}