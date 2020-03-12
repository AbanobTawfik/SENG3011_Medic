using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicApi.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Annotations;
//using Swashbuckle.AspNetCore.Examples;

namespace MedicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private Scraper _scraperService;

        public ReportsController(Scraper scraperService)
        {
            this._scraperService = scraperService;
        }


        
        [HttpGet]
        // can change routes 
        [Route("Test")]
        public ActionResult TestEndPoint()
        {
            var x = _scraperService.ScrapeData("https://www.cdc.gov/outbreaks/");
            return Ok(x);
        }


        /// <summary>
        /// Retrieves a list of articles from CDC website that fit into the criteria provided.
        /// </summary>
        /// <remarks>API Usage Information:<br/>
        /// API Returns list of articles with nested info in a JSON format. <br/>
        /// This API will return data to be used by EPIWatch Frontend.<br/>
        /// Only start time and end time is Mandatory.<br/>
        /// All other fields for query is optional and determines what is returned.<br/>
        /// 
        /// 
        /// </remarks>
        /// 
        /// <param name="start_date">Starting time of the period of interest. <example>“yyyy-MM-ddTHH:mm:ss”</example></param>
        /// 
        /// 
        /// <param name="end_date">Ending time of the period of interest. “yyyy-MM-ddTHH:mm:ss” </param>
        /// <param name="timezone">The time zone associated with the given start and end dates. Example: “AEST”</param>
        /// <param name="key_terms">Keywords for Search. Example: “Anthrax,Ebola” </param>
        /// <param name="location">The name of a location. Example: “Sydney” </param>
        /// <param name="max">Max number of reports to search. (default: 25, maximum: 50)</param>
        /// <param name="offset">The number of the first report returned. (default: 0)</param>
        /// <response code="200">Successful Query</response>
        /// <response code="500">Invalid ID supplied</response>
        /// <response code="404">Anomaly not found</response>

        // GET api/Reports/Test
        [HttpGet]
        // can change routes
        [Route("TestApi")]
        public ActionResult TestApi(string start_date,
                                    string end_date,  string timezone,
                                    string key_terms, string location,
                                    int max = 25, int offset = 0)
        {
            var service = new ReportFinder();
            var res = service.FindReports(start_date, end_date, timezone,
                                          key_terms, location, max, offset);
            return Ok(res);
        }

        [HttpGet]
        [Route("TestGenerate")]
        public ActionResult TestGenerate()
        {
            var service = new Generator();
            service.GenerateAdd();
            return Ok("done");
        }

        // GET api/Reports/TestRSS
        [HttpGet]
        [Route("TestRSS")]
        public ActionResult TestRSS()
        {
            var x = _scraperService.ScrapeOutbreaksRSS("https://tools.cdc.gov/api/v2/resources/media/285676.rss");
            return Ok(x);
        }
    }
}
