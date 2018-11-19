﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyIT.Enums;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;

namespace SurveyIT.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private IAuthService authService;
        private IAccountService accountService;


        public AccountController(IAuthService authService)
        {
            this.authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegistrationModel userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await authService.RegisterUser(userData);

            if (result)
                return Ok("Account Created");
            else
                return BadRequest("Nie zarejestrowano użytkownika");
        }


        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }



        [AllowAnonymous]
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var upuser = await accountService.UpdateUser(userData);

            if (upuser.StateMessage==CommonResultState.OK)
                return Ok("Account Updated");
            else
                return BadRequest("Nie zaktualizowano użytkownika");
        }



        [AllowAnonymous]
        [HttpPost("Auth")]
        public async Task<IActionResult> Authentication([FromBody] LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            var user = await authService.Authenticate(login);

            if (user != null)
            {
                var principal = authService.Login(user);

                await HttpContext.SignInAsync(principal);

                return Ok();
            }

            return Unauthorized();
        }
    }
}