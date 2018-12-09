using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyIT.Attributes;
using SurveyIT.Enums;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models.HelperModel;

namespace SurveyIT.Controllers
{

    [Produces("application/json")]
    [Route("api/UserManagement")]
    public class UserManagementController : Controller
    {
        private IUserManagementService userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            this.userManagementService = userManagementService;
        }

        [Auth(Role.Admin)]
        [HttpGet("DisplayAllGroups")]
        public JsonResult DisplayAllGroups()
        {
            var result = userManagementService.DisplayAllGroup();

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");
        }

        [Auth(Role.Admin)]
        [HttpGet("DisplayAllUsers")]
        public JsonResult DisplayAllUsers()
        {
            var result = userManagementService.DisplayAllUser();

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");
        }

        [Auth(Role.Admin)]
        [HttpPost("DisplayAllGroups/AssignUsers")]
        public JsonResult DisplayAssignUsers([FromQuery] string groupId)
        {
            var result = userManagementService.DisplayAssignedUsers(groupId);

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");
        }

        [Auth(Role.Admin)]
        [HttpGet("UnassignedUsers")]
        public JsonResult GetUnussignedUsers([FromQuery] string groupId)
        {
            var result = userManagementService.GetUnusignedUsers(groupId);

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");
        }

        [Auth(Role.Admin)]
        [HttpPost("AssignToGroup")]
        public async Task<IActionResult> AssignUsersToGroups([FromBody]HelperIdModelList userIdGroupId)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await userManagementService.AssignUsersToGroup(userIdGroupId.UsersId, userIdGroupId.GroupsId);

            if (result.StateMessage == CommonResultState.OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

        [Auth(Role.Admin)]
        [HttpPost("UnAssignInGroup")]
        public async Task<IActionResult> UnAssignUsersToGroups([FromBody]HelperIdModelList surveyIDGroupID)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await userManagementService.UnAssignUsersInGroup(surveyIDGroupID.UsersId, surveyIDGroupID.GroupsId);

            if (result.StateMessage == CommonResultState.OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }
    }
}