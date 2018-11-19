using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;

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

                return Ok(user.Role.ToString());
            }

            return Unauthorized();
        }
    }
}