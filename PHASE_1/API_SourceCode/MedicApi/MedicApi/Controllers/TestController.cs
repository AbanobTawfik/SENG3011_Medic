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
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("TestApi")]
        public ActionResult TestApi()
        {
            return Ok("hello world");
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
