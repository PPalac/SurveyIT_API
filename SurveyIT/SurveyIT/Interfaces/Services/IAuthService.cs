using System.Security.Claims;
using System.Threading.Tasks;
using SurveyIT.Models;
using SurveyIT.Models.DBModels;

namespace SurveyIT.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterUser(RegistrationModel userData);

        Task<Users> Authenticate(LoginModel login);

        ClaimsPrincipal Login(Users user);
    }
}
