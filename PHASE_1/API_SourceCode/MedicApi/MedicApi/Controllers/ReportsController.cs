using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MedicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        // example of GET api/Vampire
        [HttpGet]
        // can change routes 
        [Route("Test")]
        public ActionResult TestEndPoint()
        {
            return Ok("this api is working remotely");
        }

        [HttpGet]
        // can change routes 
        [Route("Test2")]
        public ActionResult TestEndPoint2()
        {
            return Ok("testing commit, i just pushed hopefully i see this");
        }
    }
}
