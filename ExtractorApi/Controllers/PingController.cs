using ExtractorApi.Helper;
using ExtractorApi.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Xml.Linq;

namespace ExtractorApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        private readonly ILogger<PingController> _logger;

        public PingController(ILogger<PingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Ping> PingApi()
        {
            return Enumerable.Range(1, 1).Select(index => new Ping
            {
                Date = DateTime.Now,
                Response = "pong" 
            }).ToArray();
        }

 
    }
}