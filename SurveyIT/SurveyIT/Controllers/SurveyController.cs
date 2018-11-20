using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyIT.Enums;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;
using SurveyIT.Models.HelperModel;

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

        //[Auth(Role.Admin)]
        [HttpGet("DisplayAll")]
        public JsonResult DisplayAll()
        {
            var result = surveyService.GetAllSurveys();

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");
        }

        //[Auth(Role.Admin)]
        [HttpPost("DisplayAll/DisplayOne")]
        public JsonResult DisplayOneSurvey([FromBody]string surveyId)
        {
            if (!ModelState.IsValid)
                return Json("Błąd wyświetlania");

            var result = surveyService.GetOneSurvey(surveyId);

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");
        }

        //[Auth(Role.Admin)]
        [HttpPost("AssignSurvey")]
        public async Task<IActionResult> AssignSurveysToGroup([FromBody]HelperIdModel surveyIDGroupID)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await surveyService.AssignSurveysToGroup(surveyIDGroupID.FirstId,surveyIDGroupID.SecondId);

            if (result.StateMessage == CommonResultState.OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

        //[Auth(Role.Admin)]
        [HttpPost("UnAssignSurvey")]
        public async Task<IActionResult> UnAssignSurveysToGroup([FromBody]HelperIdModel surveyIDGroupID)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await surveyService.UnAssignSurveysInGroup(surveyIDGroupID.FirstId, surveyIDGroupID.SecondId);

            if (result.StateMessage == CommonResultState.OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

    }
}