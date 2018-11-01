using System.ComponentModel.DataAnnotations;
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
        [HttpPost("Auth")]
        public IActionResult Authentication([FromBody] LoginModel login)
        {
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