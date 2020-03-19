using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicApi.Services;
using MedicApi.Swashbuckle;
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
        /// <summary>
        /// Retrieves a list of articles from CDC website that fit into the criteria provided.
        /// </summary>
        /// <remarks>API Usage Information:<br/>
        /// API Returns list of articles with nested info in a JSON format. <br/>
        /// This API will return data to be used by EPIWatch Frontend.<br/>
        /// Only start time and end time is Mandatory.<br/>
        /// All other fields for query is optional and determines what is returned.<br/>
        /// <heading>How it Works!</heading>
        /// 
        /// Example Query: <br/>
        /// 
        /// 
        /// Sample request:
        /// /GetArticles?start_date=2017-01-01T00:00:00&amp;end_date=2017-12-31T23: 59:59&amp;key_terms=Ebola&amp;location=Democratic%20Republic%20of%20the%20Congo
        ///
        /// Return:
        /// [
        ///    {
        ///       "url":"url1",
        ///       "reports":[
        ///          {
        ///             "diseases":[
        ///                "ebola haemorrhagic fever"
        ///             ],
        ///             "syndromes":[
        ///                "Haemorrhagic fever"
        ///             ],
        ///             "event_date":"2019-01-01 00:00:00",
        ///             "locations":[
        ///                {
        ///                   "country":"Uganda",
        ///                   "location":"Kampala"
        ///                }
        ///             ]
        ///          }
        ///       ],
        ///       "headline":"Headline 1",
        ///       "main_text":"This is the main text for article 1.",
        ///       "date_of_publication":"2019-01-02 xx:xx:xx"
        ///    }
        /// ]
        /// 
        /// </remarks>
        /// 
        /// <param name="start_date">Starting time of the period of interest. <example>“yyyy-MM-ddTHH:mm:ss”</example></param>
        /// 
        /// 
        /// <param name="end_date">Ending time of the period of interest. “yyyy-MM-ddTHH:mm:ss” </param>
        /// <param name="timezone">The time zone associated with the given start and end dates in CAPS. Example: “AEST”</param>
        /// <param name="key_terms">Keywords for Search. Example: “Anthrax,Ebola” </param>
        /// <param name="location">The name of a location. Example: “Sydney” </param>
        /// <param name="max">Max number of reports to search. (default: 25, maximum: 50)</param>
        /// <param name="offset">The number of the first report returned. (default: 0)</param>
        /// <response code="200">Successful Query with Response</response>
        /// <response code="400">Invalid Input Parameters</response>
        /// <response code="500">Internal Server Error. Try Again Later</response>
        /// <response code="404">Not Found</response>

        // GET api/Reports/GetArticles
        [HttpGet]
        // can change routes
        [Route("GetArticles")]
        [SwaggerExampleValue("start_date", "2015-01-01T00:00:00")]
        [SwaggerExampleValue("end_date", "2020-01-01T00:00:00")]
        [SwaggerExampleValue("timezone", "AEST")]
        [SwaggerExampleValue("key_terms", "anthrax,ebola,coronavirus")]
        public ActionResult GetArticles(string start_date, string end_date,
                                        string timezone, string key_terms,
                                        string location, string max,
                                        string offset)
        {
            var service = new ReportFinder();
            var errors = service.CheckRawInput(start_date, end_date, timezone,
                                               key_terms, location, max, offset);
            if (errors.NumErrors() > 0)
            {
                return BadRequest(errors);
            }

            var res = service.Retrieve(start_date, end_date, timezone,
                                       key_terms, location, max, offset);
            return Ok(res);
        }
    }
}
