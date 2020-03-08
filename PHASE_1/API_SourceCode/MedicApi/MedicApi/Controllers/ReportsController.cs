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

        // GET api/Reports/Test
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
        [Route("TestApi")]
        public ActionResult TestApi(string start_date,
                                    string end_date,  string timezone,
                                    string key_terms, string location,
                                    int max = 25, int offset = 0)
        {
            var service = new ReportFinder();
            var res = service.FindReports(start_date, end_date, timezone,
                                          key_terms, location, max, offset);
            return Ok(res);
        }

        [HttpGet]
        [Route("TestGenerate")]
        public ActionResult TestGenerate()
        {
            var service = new Generator();
            service.GenerateAdd();
            return Ok("done");
        }
    }
}
