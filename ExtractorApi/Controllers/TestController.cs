using ExtractorApi.Helper;
using ExtractorApi.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Xml.Linq;

namespace ExtractorApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Test> TestApi()
        {
            return Enumerable.Range(1, 1).Select(index => new Test
            {
                Date = DateTime.Now,
                Response = "pong"
            }).ToArray();
        }

 
    }
}