using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet]
        [Route("GeoId")]
        // GET api/Location/GeoId
        public ActionResult TestGenerate()
        {
            var service = new Generator();
            service.GenerateAdd();
            return Ok("done");
        }

        // GET api/Location/GeoName
        [HttpGet]
        [Route("GeoName")]
        public ActionResult TestRSS()
        {
            _scraperService.ScrapeAndStoreOutbreaksFromRSS("https://tools.cdc.gov/api/v2/resources/media/285676.rss");
            return Ok("done");
        }
    }
}