using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyIT.Enums;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;

namespace SurveyIT.Controllers
{
    [Produces("application/json")]
    [Route("api/Survey")]
    public class SurveyController : Controller
    {
        private ISurveyService surveyService;

        public SurveyController(ISurveyService surveyService)
        {
            this.surveyService = surveyService;
        }

        //[Auth(Role.Admin)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateSurvey([FromBody]SurveyModel survey)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await surveyService.AddSurvey(survey);

            if (result.StateMessage == CommonResultState.OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }
    }
}