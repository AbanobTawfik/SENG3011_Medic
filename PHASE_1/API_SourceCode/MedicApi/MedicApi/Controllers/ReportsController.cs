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
        /// 
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /GetArticles?start_date=2016-01-01T00%3A00%3A00&amp;end_date=2021-01-01T00%3A00%3A00&amp;key_terms=Listeria&amp;location=Arizona
        /// 
        /// Sample response:
        /// 
        ///     {
        ///       "meta": {
        ///         "team_name": "Medics",
        ///         "accessed_time": "2020-03-24T06:53:06.725Z",
        ///         "data_source": {
        ///           "name": "CDC",
        ///           "url": "https://www.cdc.gov/outbreaks/"
        ///         }
        ///       },
        ///       "articles": [
        ///         {
        ///           "url": "https://www.cdc.gov/listeria/outbreaks/enoki-mushrooms-03-20/index.html",
        ///           "reports": [
        ///             {
        ///               "diseases": [
        ///                 "listeriosis"
        ///               ],
        ///               "syndromes": [
        ///                 "Encephalitis"
        ///               ],
        ///               "event_date": "2016-11-23 xx:xx:xx to 2019-12-13 xx:xx:xx",
        ///               "locations": [
        ///                 {
        ///                   "country": "United States",
        ///                   "location": "Arizona",
        ///                   "geonames_id": 5551752
        ///                 },
        ///                 ...
        ///                 {
        ///                   "country": "United States",
        ///                   "location": "Virginia",
        ///                   "geonames_id": 6254928
        ///                 }
        ///               ]
        ///             }
        ///           ],
        ///           "headline": "Enoki Mushrooms - Listeria Infections",
        ///           "main_text": "36 people infected with the outbreak strain of Listeria monocytogenes have been reported from 17 states. ... Illnesses started on dates ranging from November 23, 2016 to December 13, 2019...",
        ///           "date_of_publication": "2020-03-01 17:40:00"
        ///         }
        ///       ]
        ///     }
        /// 
        /// Hi Kevin
        /// 
        /// </remarks>
        /// 
        /// <param name="start_date">
        ///     <p align="justify">The beginning of the date range to search. Uses the event date in reports to find matching articles. Used in conjunction with <i>end_date</i> (see explanation under <i>end_date</i>).</p>
        ///     <p><b>Format:</b> yyyy-MM-ddTHH:mm:ss</p>
        ///     <p><b>Example:</b> 2019-12-25T17:00:00</p>
        ///     <p><b>Example:</b> 2020-01-26T12:00:00</p>
        /// </param>
        /// 
        /// <param name="end_date">
        ///     <p align="justify">The end of the date range to search. Uses the event date in reports to find matching articles. Used in conjunction with <i>start_date</i>. Must not be earlier than <i>start_date</i>.</p>
        ///     <p><b>Format:</b> yyyy-MM-ddTHH:mm:ss</p>
        ///     <p align="justify"><b>Explanation:</b> The API will search for all events whose date range overlaps with the date given by the range produced by <i>start_date</i> and <i>end_date</i>, and return all articles that contain reports of these events.</p>
        ///     <p><b>Example:</b> 2020-04-25T20:00:00</p>
        ///     <p><b>Example:</b> 2020-02-19T10:00:00</p>
        /// </param>
        /// 
        /// <param name="timezone">
        ///     <p align="justify">The timezone associated with the given start and end date. Must be an unambiguous timezone abbreviation or timezone name from <a href="https://en.wikipedia.org/wiki/List_of_time_zone_abbreviations" target="_blank">this list</a>.</p>
        ///     <p align="justify"><b>Explanation:</b> Suppose <i>start_time</i> is 2020-03-23T00:00:00, <i>end_time</i> is 2020-03-23T23:59:59, and <i>timezone</i> is AEDT. The API will search for all events that occurred while it was March 23 2020 in the AEDT timezone.</p>
        ///     <p><b>Example:</b> Central Standard Time</p>
        ///     <p><b>Example:</b> AEDT</p>
        /// </param>
        /// 
        /// <param name="key_terms">
        ///     <p align="justify">The list of key terms to search for in a comma-separated string. All articles returned will match at least one of the given terms. Leave blank to retrieve all articles matching other specified criteria.</p>
        ///     <p><b>Example:</b> E. Coli</p>
        ///     <p><b>Example:</b> Listeria,Salmonella</p>
        ///     <p><b>Example:</b> Virus,Outbreak</p>
        /// </param>
        /// 
        /// <param name="location">
        ///     <p align="justify">The name of the location to search for. All articles returned will contain a report that references either the specified location or a sub-location within the specified location. Leave blank to retrieve articles matching other specified criteria.</p>
        ///     <p><b>Example:</b> Arizona</p>
        ///     <p><b>Example:</b> United States</p>
        /// </param>
        /// 
        /// <param name="max">
        ///     <p align="justify">The maximum number of matching articles to be returned (maximum: 50). If the number of matching articles is greater than <i>max</i>, the first <i>max</i> of those articles will be returned. Must be a positive integer.</p>
        ///     <p><b>Example:</b> 10</p>
        ///     <p><i>Default value</i> : 25</p>
        /// </param>
        /// 
        /// <param name="offset">
        ///     <p align="justify">The number of matching articles to skip. Must be a non-negative integer.</p>
        ///     <p align="justify"><b>Explanation:</b> Suppose <i>max</i> is 3, and the matching articles are numbered 0 through to 9. If <i>offset</i> is 0 or not specified, articles 0, 1 and 2 will be returned. If <i>offset</i> is 3, articles 3, 4 and 5 will be returned. If <i>offset</i> is 9, only article 9 will be returned.</p>
        ///     <p><b>Example:</b> 5</p>
        ///     <p><i>Default value</i> : 0</p>
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
        [SwaggerExampleValue("max", "25")]
        [SwaggerExampleValue("offset", "0")]
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
