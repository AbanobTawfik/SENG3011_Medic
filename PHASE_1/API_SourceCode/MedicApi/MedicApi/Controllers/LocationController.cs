using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicApi.Models;
using MedicApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LocationController : ControllerBase
    {
        private Scraper _scraperService;
        private ArticleRetriever _db;
        public LocationController(ArticleRetriever db)
        {
            this._db = db;
        }

        [HttpGet]
        [Route("GeoId")]
        // GET api/Location/GeoId
        public ActionResult TranslateGeoId([FromBody] string geoId)
        {
            var ret = _db.GetLocationIfExists(geoId);
            if (ret == null)
            {
                return Ok("NO RESULT");
            }
            else
            {
                return Ok(ret);
            }
        }

        [HttpGet]
        [Route("GeoName")]
        // GET api/Location/GeoName
        public ActionResult TranslateGeoName([FromBody] string country, string location)
        {
            var ret = _db.GetLocationIfExists(country, location);
            if (ret == null)
            {
                return Ok("NO RESULT");
            }
            else
            {
                return Ok(ret);
            }
        }

        [HttpPost]
        [Route("AddLocation")]
        // POST api/Location/AddLocation
        public async Task<ActionResult> AddLocationAsync([FromBody] FrontEndLocation location)
        {
            _db.AddLocation(location);
            return Ok();
        }
    }
}