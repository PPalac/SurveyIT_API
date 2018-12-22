using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyIT.Attributes;
using SurveyIT.Enums;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;
using SurveyIT.Models.HelperModel;

namespace SurveyIT.Controllers
{
    [Produces("application/json")]
    [Route("api/Group")]
    public class GroupController : Controller
    {
        private IGroupService groupService;

        public GroupController(IGroupService groupService)
        {
            this.groupService = groupService;
        }

        [Auth(Role.Admin)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateGroup([FromBody]GroupModel group)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await groupService.AddGroup(group);

            if (result.StateMessage == CommonResultState.OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

        [Auth(Role.Admin)]
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteGroup([FromBody]GroupModel group)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await groupService.DeleteGroup(group.Name);

            if (result.StateMessage == CommonResultState.OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

        [Auth(Role.Admin)]
        [HttpPost("Edit")]
        public async Task<IActionResult> EditGRoup([FromBody]HelperNewGroupModel helperNewGroup) //todo: dodac id do groupmodel
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await groupService.EditGroup(helperNewGroup.groupModel, helperNewGroup.newNameGroup);

            if (result.StateMessage == CommonResultState.OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

        [Auth(Role.Admin)]
        [HttpGet("Display")]
        public JsonResult DisplayAll()
        {
            var result = groupService.GetAllGroups();

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");
        }

        [Auth(Role.Admin)]
        [HttpPost("Display/OneGroup")]
        public JsonResult DisplayOneGroup([FromBody]string groupId)
        {
            var result = groupService.GetOneGroup(groupId);

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");

        }

        [Auth(Role.Admin)]
        [HttpGet("GroupsForSurvey")]
        public IActionResult GetGroupsForSurvey([FromQuery] int surveyId)
        {
            var result = groupService.GetGroupsForSurvey(surveyId);

            return Ok(result);
        }
    }
}
