using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace MedicApi.Swashbuckle
{
    /// <summary>
    /// A custom attribute for setting default parameter values in Swagger.
    /// Decorate the desired API endpoint '[SwaggerExampleValue(name, value)]'.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerExampleValue : Attribute
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public SwaggerExampleValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    /// <summary>
    /// A Swashbuckle filter that applies the default values.
    /// Add 'c.OperationFilter/<AddExampleValues/>' to AddSwaggerGen in Startup
    /// </summary>
    public class AddExampleValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Exit if the endpoint takes no parameters
            if (operation.Parameters == null || !operation.Parameters.Any())
                return;
            // Iterate through the attributes of the endpoint (context)
            foreach (var attribute in context.ApiDescription.CustomAttributes().OfType<SwaggerExampleValue>())
            {
                // Find the corresponding Swagger parameter
                var parameter = operation.Parameters.First(p => p.Name == attribute.Name);
                // Set the parameter's example value if found
                if (parameter != null)
                    parameter.Schema.Example = new OpenApiString(attribute.Value);
            }
        }
    }
}
