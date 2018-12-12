using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyIT.Attributes;
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

        [Auth(Role.Admin)]
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

        [Authorize]
        [HttpGet("DisplayAll")]
        public IActionResult DisplayAll()
        {

            if (HttpContext.User.FindFirstValue(ClaimTypes.Role) == Role.User.ToString())
                return RedirectToAction("DisplayNotFillSurveys", "Account");

            var result = surveyService.GetAllSurveys();

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");
        }

        [Authorize]
        [HttpGet("DisplayAll/DisplayOne")]
        public JsonResult DisplayOneSurvey([FromQuery] string surveyId)
        {
            if (!ModelState.IsValid)
                return Json("Błąd wyświetlania");

            var result = surveyService.GetOneSurvey(surveyId);

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");
        }

        [Auth(Role.Admin)]
        [HttpPost("AssignSurvey")]
        public async Task<IActionResult> AssignSurveysToGroup([FromBody]HelperIdModelList surveyIDGroupID)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await surveyService.AssignSurveysToGroup(surveyIDGroupID.UsersId, surveyIDGroupID.GroupsId);

            if (result.StateMessage == CommonResultState.OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

        [Auth(Role.Admin)]
        [HttpPost("UnAssignSurvey")]
        public async Task<IActionResult> UnAssignSurveysToGroup([FromBody]HelperIdModelList surveyIDGroupID)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await surveyService.UnAssignSurveysInGroup(surveyIDGroupID.UsersId, surveyIDGroupID.GroupsId);

            if (result.StateMessage == CommonResultState.OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

        [Auth(Role.User)]
        [HttpPost("FillSurvey")]
        public async Task<IActionResult> FillSurvey([FromBody]HelperFillSurveyModel fillSurveyModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await surveyService.FillSurvey(fillSurveyModel.surveyId, fillSurveyModel.UserAnswerModel, userId);

            if (result.StateMessage == CommonResultState.OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }
    }
}