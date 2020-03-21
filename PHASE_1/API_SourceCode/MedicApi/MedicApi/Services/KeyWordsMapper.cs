using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicApi.Services
{
    public class KeyWordsMapper: Mapper
    {
        public KeyWordsMapper(List<string> keys) : base(keys)
        {
            base.AddKey("MapperKeyWords");
        }

        public void AddKeysFromOtherMappers(List<Mapper> mappers)
        {
            foreach (var mapper in mappers)
            {
                foreach (var key in mapper.AllReferences())
                {
                    base.AddReference("MapperKeyWords", key);
                }
            }            
        }
    }
}
