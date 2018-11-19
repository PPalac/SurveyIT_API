﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyIT.Attributes;
using SurveyIT.Enums;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;

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
        public async Task<IActionResult> EditGRoup([FromBody]GroupModel group, string newGroupName) //todo: dodac id do groupmodel
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await groupService.EditGroup(group, newGroupName);

            if (result.StateMessage == CommonResultState.OK)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

        [Auth(Role.Admin)]
        [HttpGet("Display")]
        public async Task<IActionResult> DisplayAll()
        {
            var result = groupService.GetAllGroups();
            
            if (result != null)
                return Ok(Json(result));

            return BadRequest("Błąd wyświetlania");

        }

        [Auth(Roles = "Admin")]
        [HttpGet("Display/OneGroup")]
        public async Task<IActionResult> DisplayOneGroup([FromBody]string groupId)
        {
            var result = groupService.GetOneGroup(groupId);

            if (result != null)
                return Ok(Json(result));

            return BadRequest("Błąd wyświetlania");

        }
    }
}
