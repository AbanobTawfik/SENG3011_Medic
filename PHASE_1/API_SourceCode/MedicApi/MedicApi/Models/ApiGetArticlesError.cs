using System;
using System.Collections.Generic;
using System.Text;

namespace MedicApi.Models
{
    public class ApiGetArticlesError
    {
        public ApiResponseMetadata meta { get; set; }
        public Dictionary<string, string> errors { get; set; }

        public ApiGetArticlesError(DateTime accessed_time)
        {
            meta = new ApiResponseMetadata(accessed_time);
            errors = new Dictionary<string, string>();
        }

        public void AddError(string field, string message)
        {
            errors.Add(field, message);
        }

        public int NumErrors()
        {
            return errors.Count;
        }
    }
}
