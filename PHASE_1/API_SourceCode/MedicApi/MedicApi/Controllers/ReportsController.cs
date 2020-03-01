using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicApi.Services;
using Microsoft.AspNetCore.Mvc;

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

        // example of GET api/Vampire
        [HttpGet]
        // can change routes 
        [Route("Test")]
        public ActionResult TestEndPoint()
        {
            var x = _scraperService.ScrapeData("https://www.cdc.gov/outbreaks/");
            return Ok(x);
        }

        [HttpGet]
        // can change routes 
        [Route("Test2")]
        public ActionResult TestEndPoint2()
        {
            return Ok("removed payed app service");
        }
    }
}
