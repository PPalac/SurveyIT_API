using System.Threading.Tasks;
using SurveyIT.Models;
using SurveyIT.Models.DBModels;

namespace SurveyIT.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterUser(RegistrationModel userData);

        Task<Users> Authenticate(LoginModel login);

        string Buildtoken(Users user);
    }
}
