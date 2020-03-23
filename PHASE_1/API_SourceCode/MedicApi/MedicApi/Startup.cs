using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MedicApi.Services;
using MedicApi.Swashbuckle;
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
            var symptoms = new List<string>();
            Configuration.GetSection("Syndromes").Bind(syndromes);
            Configuration.GetSection("Syndromes").Bind(symptoms);
            var conjunctions = new List<string>();
            Configuration.GetSection("Conjunctions").Bind(conjunctions);
            var keywords = new List<string>();
            Configuration.GetSection("Keywords").Bind(keywords);
            var diseaseMapper = new DiseaseMapper(diseases);
            var syndromeMapper = new SyndromeMapper(syndromes);
            var symptomMapper = new SymptomMapper(symptoms);
            var keywordsMapper = new KeyWordsMapper(keywords);
            keywordsMapper.AddKeysFromOtherMappers(new List<Mapper>
            {
                diseaseMapper,
                symptomMapper,
                syndromeMapper
            });
            var locationMapper = new LocationMapper(AppContext.BaseDirectory);
            var scraper = new Scraper(diseaseMapper, syndromeMapper, symptomMapper, keywordsMapper, locationMapper, conjunctions);
            var x = new APILogger();
            services.AddSingleton<APILogger>();
            services.AddSingleton<ArticleRetriever>();
            services.AddSingleton(diseaseMapper);
            services.AddSingleton(syndromeMapper);
            services.AddSingleton(scraper);
            var asd = new Microsoft.OpenApi.Models.OpenApiInfo();
            // swagger generation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Medics CDC Articles API",
                    Description = "<div align=\"justify\">This API is developed for a new system that automates the extraction of disease outbreak data from the US Department of Health’s Centers for Disease Control and Prevention (CDC) website.<br/>Access this API at seng3011medics.azurewebsites.net/api/reports/GetArticles with the various input parameters per definitions below.</div>",
                    Version = "v1"
                });
                c.OperationFilter<AddExampleValues>();
                c.OperationFilter<AddReportsFilter>();
                c.SchemaFilter<AddSchemaFilter>();

                var filePath = Path.Combine(AppContext.BaseDirectory, "MedicApi.xml");
                c.IncludeXmlComments(filePath);
            }
            );
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
