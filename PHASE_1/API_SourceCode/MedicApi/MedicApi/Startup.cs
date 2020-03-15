using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MedicApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
namespace MedicApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            var diseases = new List<string>();
            Configuration.GetSection("Diseases").Bind(diseases);
            var syndromes = new List<string>();
            Configuration.GetSection("Syndromes").Bind(syndromes);
            var diseaseMapper = new DiseaseMapper(diseases);
            var syndromeMapper = new SyndromeMapper(syndromes);
            var scraper = new Scraper(diseaseMapper, syndromeMapper);
            services.AddSingleton(diseaseMapper);
            services.AddSingleton(syndromeMapper);
            services.AddSingleton(scraper);
            // swagger generation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Medics SENG3011 CDC Articles API",
                    Description = "This API is an endpoint to retrieve all outbreak articles.<br/> This API is developed for a new system that automates the extraction of latest outbreaks data from the US health department’s Centers for Disease Control and Prevention (CDC) website. <br/> Query parameters will be extracted from the request and used to filter through the database and retrieve articles that match the query. The resulting list of articles is then returned to the user in an 200 response. <br/> If any of the query parameters are invalid or any of the dates are missing, we will return a 400 Request response alongside an error message to inform the user of the issue. <br/>API can be accessed at https://seng3011medics-staging.azurewebsites.net/api/reports/TestApi with the various input parameters per definitions below.",
                    Version = "0.01"
                });
                
                var filePath = Path.Combine(AppContext.BaseDirectory, "MedicApi.xml");
                c.IncludeXmlComments(filePath);
            }
            ) ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core API");
            });
        }
    }
}
