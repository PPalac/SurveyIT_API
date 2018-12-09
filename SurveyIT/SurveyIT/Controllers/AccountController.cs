using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyIT.Attributes;
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

        public AccountController(IAuthService authService, IAccountService accountService)
        {
            this.authService = authService;
            this.accountService = accountService;
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
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var upuser = await accountService.UpdateUser(userData);

            if (upuser.StateMessage == CommonResultState.OK)
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

                return Ok(user.Role.ToString());
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [Auth(Role.Admin)]
        [HttpGet("DisplayUsers")]
        public JsonResult DisplayAllUser()
        {
            var result = accountService.GetAllUsersWithRoleUser();

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");
        }

        [Auth(Role.Admin)]
        [HttpPost("DisplayUsers/User")]
        public JsonResult DisplayOneUser([FromBody]string userId)
        {
            if (!ModelState.IsValid)
            {
                return Json("Błąd wyświetlania");
            }

            var result = accountService.GetOneUserById(userId);

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");

        }

        //Te metody do zmiany, bo mi logowanie nie dziala idk czemu, to musisz zamienic post na get i dac ta metode co wyciaga Ci usera (da sie zeby wyciagnelo id?)
        //Bo wtedy nie musialbym zmieniac reszty metod gdzie przekazuje id a nie username
        [Authorize]
        [HttpPost("DisplayUsers/User/AllSurveys/NotFillSurvey")]
        public JsonResult DisplayNotFillSurveys()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = accountService.GetAllNotFillSurvey(userId);

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");

        }

        [Authorize]
        [HttpPost("DisplayUsers/User/AllSurveys/NotFillSurvey/GetOne")]
        public JsonResult DisplayOneNotFillSurveys([FromQuery]string surveyId)
        {
            var result = accountService.GetOneNotFillSurvey(surveyId);

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");

        }

        [HttpPost("DisplayUsers/User/AllSurveys/FillSurvey")]
        public JsonResult DisplayFillSurveys()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = accountService.GetAllFillSurvey(userId);

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");

        }

        [Authorize]
        [HttpPost("DisplayUsers/User/AllSurveys/FillSurvey/GetOne")]
        public JsonResult DisplayOneFillSurveys([FromQuery] string surveyId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = accountService.GetOneFillSurvey(userId, surveyId);

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");

        }

        [Auth(Role.User)]
        [HttpGet("DisplayUsers/User/AllSurveys/NotFillSurveyAfterDate")]
        public JsonResult DisplayNotFillSurveysAfterDate()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = accountService.GetAllNotFillSurveyAfterDate(userId);

            if (result != null)
                return Json(result);

            return Json("Błąd wyświetlania");

        }

        [Auth(Role.Admin)]
        [HttpGet("CurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var username = HttpContext.User.Identities.First().Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(username))
            {
                var result = await accountService.GetUserByUsername(username);

                if (result != null)
                {
                    return Ok(Json(result));
                }
            }

            return BadRequest("Nie znaleziono użytkownika");
        }
    }
}
