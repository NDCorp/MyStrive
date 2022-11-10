using ExtractorApi.Helper;
using ExtractorApi.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Xml.Linq;

namespace ExtractorApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseEvaluationController : ControllerBase
    {
        private readonly ILogger<CourseEvaluationController> _logger;

        public CourseEvaluationController(ILogger<CourseEvaluationController> logger)
        {
            _logger = logger;
        }

        // GET: /CourseEvaluation/F2022
        [HttpGet("{term}")]
        public async Task<IActionResult> GetAllSyllabusData([FromRoute] string term)
        {
            try
            {
                var helper = new ExtractorHelper();
                var allData = await helper.MergeData(term);

                if (allData?.Any() == true)
                    return Ok(allData);
                else
                    return NotFound(allData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }

        }

        // POST: /CourseEvaluation
        [HttpPost]
        public async Task<IActionResult> PostSyllabusData([FromBody] CourseEvaluation CourseData)
        {
            try
            {
                var helper = new ExtractorHelper();
                await helper.SaveData(CourseData);
                return CreatedAtAction("GetAllSyllabusData", new { term = CourseData.Term }, CourseData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }


    }
}