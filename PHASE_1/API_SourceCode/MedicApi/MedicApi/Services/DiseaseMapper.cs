using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicApi.Services
{
    public class DiseaseMapper:Mapper
    {
        public DiseaseMapper(List<string> keys) : base(keys)
        {
            base.AddReference("unknown disease", "unknown diseases");
            base.AddReference("anthrax cutaneous", "cutaneous anthrax");
            base.AddReference("anthrax gastrointestinous", "gastrointestinal anthrax");
            base.AddReference("anthrax inhalation", "inhalation anthrax");
            base.AddReference("botulism", "clostridium botulinum"); // <-- the bacteria
            base.AddReference("brucellosis", "undulant fever");
            base.AddReference("brucellosis", "undulating fever");
            base.AddReference("brucellosis", "mediterranean fever");
            base.AddReference("brucellosis", "malta fever");
            base.AddReference("brucellosis", "cyprus fever");
            base.AddReference("brucellosis", "rock fever");
            base.AddReference("brucellosis", "brucella"); // <-- the bacteria
            base.AddReference("chikungunya", "chikv"); // <-- the virus
            base.AddReference("cholera", "vibrio cholerae"); // <-- the bacteria
            base.AddReference("cryptococcosis", "cryptococcus"); // <-- the fungus
            base.AddReference("cryptosporidiosis", "cryptosporidium"); // <-- the parasite
            base.AddReference("crimean-congo haemorrhagic fever", "cchf");
            base.AddReference("dengue", "breakbone");
            base.AddReference("diphtheria", "corynebacterium diphtheriae"); // <-- the bacteria
            base.AddReference("ebola haemorrhagic fever", "ebola");
            base.AddReference("ebola haemorrhagic fever", "evd");
            base.AddReference("ehec (e.coli)", "ehec");
            base.AddReference("ehec (e.coli)", "e. coli");
            base.AddReference("ehec (e.coli)", "escherichia coli");
            base.AddReference("ehec (e.coli)", "ecolidisease");
            base.AddReference("enterovirus 71 infection", "enterovirus 71"); // <-- the virus
            base.AddReference("enterovirus 71 infection", "enterovirus a71"); // <-- the virus
            base.AddReference("enterovirus 71 infection", "ev71"); // <-- the virus
            base.AddReference("influenza a/h5n1", "h5n1");
            base.AddReference("influenza a/h7n9", "h7n9");
            base.AddReference("influenza a/h9n2", "h9n2");
            base.AddReference("influenza a/h1n1", "h1n1");
            base.AddReference("influenza a/h3n5", "h3n5");
            base.AddReference("influenza a/h3n2", "h3n2");
            base.AddReference("hand, foot and mouth disease", "hfmd");
            base.AddReference("hantavirus", "orthohantavirus");
            base.AddReference("hepatitis a", "hep a");
            base.AddReference("hepatitis a", "hav");
            base.AddReference("hepatitis b", "hep b");
            base.AddReference("hepatitis b", "hbv");
            base.AddReference("hepatitis c", "hep c");
            base.AddReference("hepatitis c", "hcv");
            base.AddReference("hepatitis d", "hepatitis delta");
            // hepatitis e
            base.AddReference("histoplasmosis", "histoplasma");
            base.AddReference("hiv/aids", "hiv");
            base.AddReference("hiv/aids", "aids");
            base.AddReference("lassa fever", "lassa haemorrhagic fever");
            base.AddReference("lassa fever", "lhf");
            base.AddReference("malaria", "plasmodium");
            base.AddReference("marburg virus disease", "mvd");
            base.AddReference("measles", "rubeola");
            base.AddReference("mers-cov", "middle east respiratory syndrome coronavirus");
            base.AddReference("mumps", "parotitis");
            // nipah virus
            base.AddReference("norovirus infection", "norovirus");
            base.AddReference("pertussis", "whooping cough");
            // plague
            base.AddReference("pneumococcus pneumonia", "pneumococcal pneumonia");
            base.AddReference("pneumococcus pneumonia", "pneumococcal disease");
            base.AddReference("poliomyelitis", "polio");
            base.AddReference("q fever", "query fever");
            // rabies
            base.AddReference("rift valley fever", "rvf");
            // rotavirus
            base.AddReference("rubella", "german measles");
            base.AddReference("salmonellosis", "salmonella");
            base.AddReference("sars", "severe acute respiratory syndrome");
            base.AddReference("shigellosis", "shigella"); // shigella is the bacteria
            base.AddReference("smallpox", "variola");
            base.AddReference("staphylococcal enterotoxin b", "seb"); // seb is a toxin
            base.AddReference("typhoid fever", "enteric fever");
            // tuberculosis
            base.AddReference("tularemia", "rabbit fever");
            base.AddReference("vaccinia and cowpox", "cowpox");
            base.AddReference("varicella", "chicken pox");
            base.AddReference("varicella", "chickenpox");
            base.AddReference("west nile virus", "wnv");
            // yellow fever
            base.AddReference("yersiniosis", "yernisia");
            // zika
            base.AddReference("legionnaires", "legionnaires disease");
            base.AddReference("legionnaires", "legionnaires' disease");
            base.AddReference("legionnaires", "legionellosis");
            base.AddReference("legionnaires", "legionella"); // legionella is the bacteria
            base.AddReference("listeriosis", "listeria"); // listeria is the bacteria
            // monkeypox
            base.AddReference("COVID-19", "coronavirus");
            base.AddReference("COVID-19", "2019-ncov");
        }
    }
}
