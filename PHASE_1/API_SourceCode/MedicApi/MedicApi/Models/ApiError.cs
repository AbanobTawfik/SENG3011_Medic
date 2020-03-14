using System;
using System.Collections.Generic;
using System.Text;

namespace MedicApi.Models
{
    public class ApiError
    {
        public Dictionary<string, string> errors;

        public ApiError()
        {
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
