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
                operation.Parameters.First(p => p.Name == "start_date").Required = true;
                operation.Parameters.First(p => p.Name == "end_date").Required = true;
                operation.Parameters.First(p => p.Name == "timezone").Schema.Default = new OpenApiString("UTC");
            }
        }
    }
}
