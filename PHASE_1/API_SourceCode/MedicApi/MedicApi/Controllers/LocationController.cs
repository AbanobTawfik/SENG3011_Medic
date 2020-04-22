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

        [HttpPost]
        [Route("GeoId")]
        // GET api/Location/GeoId
        public ActionResult TranslateGeoId([FromBody] GeoId geoid)
        {
            var ret = _db.GetLocationIfExists(geoid.geoId);
            if (ret == null)
            {
                var err = new { error = "NO RESULT" };
                return Ok(err);
            }
            else
            {
                return Ok(ret);
            }
        }

        [HttpPost]
        [Route("GeoName")]
        // GET api/Location/GeoName
        public ActionResult TranslateGeoName([FromBody] Location place)
        {
            var ret = _db.GetLocationIfExists(place.country, place.location);
            if (ret == null)
            {
                var err = new { error = "NO RESULT" };
                return Ok(err);
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