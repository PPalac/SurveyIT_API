using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurveyIT.DB;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;
using SurveyIT.Models.DBModels;

namespace SurveyIT.Controllers
{
    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private IAuthService authService;

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

        [AllowAnonymous]
        [HttpPost("Auth")]
        public IActionResult Authentication([FromBody] LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = authService.Authenticate(login);

            if (user != null)
            {
                var token = authService.Buildtoken(user);

                return Ok(token);
            }

            return Unauthorized();
        }
    }
}