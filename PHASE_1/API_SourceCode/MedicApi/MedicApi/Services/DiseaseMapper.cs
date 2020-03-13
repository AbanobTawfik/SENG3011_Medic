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
        }
    }
}
