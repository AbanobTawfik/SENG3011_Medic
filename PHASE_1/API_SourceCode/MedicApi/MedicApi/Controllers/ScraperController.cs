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
    public class ScraperController : ControllerBase
    {
        private Scraper _scraperService;

        public ScraperController(Scraper scraperService)
        {
            this._scraperService = scraperService;
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