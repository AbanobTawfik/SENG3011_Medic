using System;
using System.Collections.Generic;
using System.Diagnostics;
using MedicApi.Models;
using MedicApi.Services;
using MedicApi.Swashbuckle;
using Microsoft.AspNetCore.Mvc;
//using Swashbuckle.AspNetCore.Examples;

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
        ///     Retrieves a list of articles from CDC website that match the given criteria.
        /// </summary>
        /// <remarks>API Usage Information:<br/>
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
        /// Sample request:
        /// 
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
        /// 
        /// </remarks>
        /// 
        /// <param name="start_date">
        ///     REQUIRED Starting date & time of the report event dates.
        ///     <example>“yyyy-MM-ddTHH:mm:ss”</example>
        /// </param>
        /// 
        /// <param name="end_date">
        ///     REQUIRED Ending date & time of the report event dates.
        ///     “yyyy-MM-ddTHH:mm:ss”
        /// </param>
        /// 
        /// <param name="timezone">
        ///     The time zone associated with the given start and end dates in CAPS.
        ///     Default: “UTC”
        /// </param>
        /// 
        /// <param name="key_terms">
        ///     Keywords for Search.
        ///     Example: “Anthrax,Ebola”
        /// </param>
        /// 
        /// <param name="location">
        ///     The name of a location.
        ///     Example: “Sydney”
        /// </param>
        /// 
        /// <param name="max">
        ///     Max number of reports to search. (default: 25, maximum: 50)
        /// </param>
        /// 
        /// <param name="offset">
        ///     The number of the first report returned. (default: 0)
        /// </param>
        /// 
        /// <response code="200">Successful Query with Response</response>
        /// <response code="400">Invalid Input Parameters</response>
        /// <response code="500">Internal Server Error. Try Again Later</response>
        /// <response code="404">Not Found</response>

        [Route("GetArticles")]
        [SwaggerExampleValue("start_date", "2015-01-01T00:00:00")]
        [SwaggerExampleValue("end_date", "2020-01-01T00:00:00")]
        [SwaggerExampleValue("timezone", "UTC")]
        [SwaggerExampleValue("key_terms", "anthrax,ebola,coronavirus")]
        [ProducesResponseType(typeof(ApiGetArticlesResponse), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [HttpGet]
        public ActionResult GetArticles([FromQuery]string start_date,
                                        [FromQuery]string end_date,
                                        [FromQuery]string timezone,
                                        [FromQuery]string key_terms,
                                        [FromQuery]string location,
                                        [FromQuery]string max,
                                        [FromQuery]string offset)
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
