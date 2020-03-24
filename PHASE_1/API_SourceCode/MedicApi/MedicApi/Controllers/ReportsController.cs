using System;
using System.Collections.Generic;
using System.Diagnostics;
using MedicApi.Models;
using MedicApi.Services;
using MedicApi.Swashbuckle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        protected APILogger _logger;
        private ArticleRetriever _db;

        public ReportsController(APILogger logger, ArticleRetriever db)
        {
            this._logger = logger;
            this._db = db;
        }

        /// <summary>
        /// Finds articles containing disease reports that match the given criteria.
        /// </summary>
        /// <remarks>
        /// API Usage Information:<br/>
        /// 
        /// This endpoint will act as a search engine for querying outbreaks on the CDC site. <br/>
        /// Query parameters will be extracted from the request and used to filter through the database and retrieve articles that match the query. <br/>
        /// The resulting list of articles is then returned to the user in an 200 response. <br/>
        /// If any of the query parameters are invalid or any of the dates are missing, a 400 Bad Request response will be returned alongside an error message to inform the user of the issue.<br/>
        /// API Returns list of articles with nested info in a JSON format.<br/>
        /// This API will return data to be used by EPIWatch Frontend.<br/>
        /// Only start time and end time is Mandatory.<br/>
        /// All other fields for query is optional and determines what is returned.<br/>
        /// <heading>How it Works!</heading>
        /// Example Query:<br/>
        /// 
        /// Sample request:
        /// 
        ///     GET /GetArticles?start_date=2016-01-01T00:00:00&amp;end_date=2021-01-01T00:00:00&amp;key_terms=Listeria&amp;location=Arizona
        /// 
        /// Sample response:
        /// 
        ///     [
        ///       {
        ///         "url":"https://www.cdc.gov/listeria/outbreaks/enoki-mushrooms-03-20/index.html",
        ///         "reports":[
        ///           {
        ///             "diseases":[
        ///               "listeriosis"
        ///             ],
        ///             "syndromes":[
        ///               "Encephalitis"
        ///             ],
        ///               "event_date": "2016-11-23 xx:xx:xx to 2019-12-13 xx:xx:xx",
        ///             "locations": [
        ///               {
        ///                 "country": "United States",
        ///                 "location": "Arizona",
        ///                 "geonames_id": 5551752
        ///               },
        ///               ...
        ///               {
        ///                 "country": "United States",
        ///                 "location": "Virginia",
        ///                 "geonames_id": 6254928
        ///               }
        ///             ]
        ///           }
        ///         ], 
        ///         "headline":"Enoki Mushrooms - Listeria Infections",
        ///         "main_text":"36 people infected with the outbreak strain of Listeria monocytogenes have been reported from 17 states.
        ///                      Illnesses started on dates ranging from November 23, 2016 to December 13, 2019...",
        ///         "date_of_publication":"2020-03-01 17:40:00"
        ///       }
        ///     ]
        /// 
        /// </remarks>
        /// 
        /// <param name="start_date">
        ///     The earliest date that a report can be recorded on.
        ///     <example>Format: “yyyy-MM-ddTHH:mm:ss”</example>
        /// </param>
        /// 
        /// <param name="end_date">
        ///     The latest date that a report can be recorded on.
        ///     Format: “yyyy-MM-ddTHH:mm:ss”
        /// </param>
        /// 
        /// <param name="timezone">
        ///     The timezone associated with the given start and end dates in CAPS.
        /// </param>
        /// 
        /// <param name="key_terms">
        ///     Key terms to search for, in a comma-separated string.
        /// </param>
        /// 
        /// <param name="location">
        ///     The location a report can refer to.
        /// </param>
        /// 
        /// <param name="max">
        ///     A limit on the number of articles returned (maximum: 50).
        /// </param>
        /// 
        /// <param name="offset">
        ///     The number of entries to skip in the list of returned articles.
        /// </param>
        /// 
        /// <response code="200">Successful query</response>
        /// <response code="400">Invalid input parameters</response>
        /// <response code="500">Internal server error</response>
        [Route("GetArticles")]
        [SwaggerExampleValue("start_date", "2016-01-01T00:00:00")]
        [SwaggerExampleValue("end_date", "2021-01-01T00:00:00")]
        [SwaggerExampleValue("key_terms", "Listeria")]
        [SwaggerExampleValue("location", "Arizona")]
        [ProducesResponseType(typeof(ApiGetArticlesResponse), 200)]
        [ProducesResponseType(typeof(ApiGetArticlesError), 400)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [HttpGet]
        public ActionResult GetArticles([FromQuery]string start_date,
                                        [FromQuery]string end_date,
                                        [FromQuery]string timezone,
                                        [FromQuery]string key_terms,
                                        [FromQuery]string location,
                                        [FromQuery]string max = "25",
                                        [FromQuery]string offset = "0")
        {
            DateTime accessed_time = DateTime.Now;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            this._logger.LogReceive(start_date, end_date, timezone, key_terms, location, max, offset);

            // check for errors
            var err = new ApiGetArticlesError(accessed_time);
            _db.CheckRawInput(err, start_date, end_date, timezone,
                              key_terms, location, max, offset);
            if (err.NumErrors() > 0)
            {
                stopWatch.Stop();
                var TimeTakenForError = stopWatch.Elapsed.ToString();
                this._logger.LogErrors(err, TimeTakenForError);
                return BadRequest(err);
            }

            // retrieve articles
            List<Article> articles = _db.Retrieve(start_date, end_date,
                                                  timezone, key_terms, location,
                                                  max, offset);
            var res = new ApiGetArticlesResponse(accessed_time, articles);
            stopWatch.Stop();
            var TimeTakenForSuccess = stopWatch.Elapsed.ToString();
            this._logger.LogSuccess(articles.ToArray(), TimeTakenForSuccess);
            return Ok(res);
        }
    }
}
