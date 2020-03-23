using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using MedicApi.Models;
using MedicApi.Services;

namespace MedicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TestController : ControllerBase
    {
        private ArticleRetriever _db;

        public TestController(ArticleRetriever db)
        {
            this._db = db;
        }

        [HttpGet]
        [Route("GetArticles")]
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

            var err = new ApiGetArticlesError(accessed_time);
            _db.CheckRawInput(err, start_date, end_date, timezone,
                              key_terms, location, max, offset);
            if (err.NumErrors() > 0)
            {
                return BadRequest(err);
            }

            _db.SetTesting(true);
            List<Article> articles = _db.Retrieve(start_date, end_date,
                                                  timezone, key_terms, location,
                                                  max, offset);
            _db.SetTesting(false);

            var res = new ApiGetArticlesResponse(accessed_time, articles);
            return Ok(res);
        }

        [HttpGet]
        [Route("LoadTestData")]
        public ActionResult LoadTestData()
        {
            TestDataLoader.LoadTestData();
            return Ok("done");
        }
    }
}
