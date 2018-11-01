using SurveyIT.Models;

namespace SurveyIT.Interfaces.Services
{
    public interface IAuthService
    {
        UserModel Authenticate(LoginModel login);

        string Buildtoken(UserModel user);
    }
}
