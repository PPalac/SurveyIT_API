﻿using Microsoft.AspNetCore.Mvc;
using SurveyIT.Attributes;
using SurveyIT.Enums;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        //[Auth(Role.Admin)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateGroup([FromBody]GroupModel group)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await groupService.AddGroup(group);

            if (result.StateMessage == CommonResultState.OK)
                return Ok("Grupa dodana");

            return BadRequest("Błąd");
        }

        //[Auth(Role.Admin)]
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteGroup([FromBody]GroupModel group)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await groupService.DeleteGroup(group.Name);

            if (result.StateMessage == CommonResultState.OK)
                return Ok("Grupa usunieta");

            return BadRequest("Błąd");
        }

        //[Auth(Role.Admin)]
        [HttpPost("Edit")]
        public async Task<IActionResult> EditGRoup([FromBody]GroupModel group, string newGroupName) //todo: dodac id do groupmodel
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await groupService.EditGroup(group, newGroupName);

            if (result.StateMessage == CommonResultState.OK)
                return Ok("Nazwa została zmieniona");

            return BadRequest("Błąd");
        }

        //[Auth(Role.Admin)]
        [HttpGet("Display")]
        public JsonResult DisplayAll() //todo: return lista groupmodel
        {
            var result = groupService.GetAllGroups();
            
            if (result != null)
                return Json(result);

            return Json("");

        }
    }
}
