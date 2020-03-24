using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace MedicApi.Swashbuckle
{
    public class AddReportsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.RelativePath == "api/Reports/GetArticles")
            {
                operation.Parameters[0].Required = true;
                operation.Parameters[1].Required = true;
                operation.Parameters[2].Schema.Default = new OpenApiString("UTC");
            }
        }
    }
}
