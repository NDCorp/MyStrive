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
        private readonly IExtractorHelper _extractorHelper;

        public CourseEvaluationController(ILogger<CourseEvaluationController> logger, IExtractorHelper extractorHelper)
        {
            _logger = logger;
            _extractorHelper = extractorHelper;
        }

        // GET: /CourseEvaluation/F2022
        [HttpGet("{term}")]
        public async Task<IActionResult> GetAllSyllabusData([FromRoute] string term)
        {
            try
            {
                //var helper = new ExtractorHelper();
                //var allData = await helper.MergeData(term);
                var allData = await _extractorHelper.MergeData(term);

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
        public async Task<IActionResult> PostSyllabusData([FromBody] CourseEvaluation courseData)
        {
            try
            {
                //var helper = new ExtractorHelper();
                //await helper.SaveData(CourseData);
                await _extractorHelper.SaveData(courseData);
                return CreatedAtAction("GetAllSyllabusData", new { term = courseData.Term }, courseData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }


    }
}