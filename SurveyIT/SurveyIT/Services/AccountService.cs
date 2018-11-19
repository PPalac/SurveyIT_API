using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SurveyIT.DB;
using SurveyIT.Enums;
using SurveyIT.Helpers;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;
using SurveyIT.Models.DBModels;


namespace SurveyIT.Services
{
    public class AccountService : IAccountService
    {
        private UserManager<Users> userManager;
        private MyDbContext dbContext;

        public AccountService(UserManager<Users> userManager, MyDbContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        private CommonResult CheckUser(UserModel user)
        {
            if (user != null)
            {
                if ((Regex.IsMatch(user.FirstName.First().ToString(), "[A-Z]")) && (Regex.IsMatch(user.LastName.First().ToString(), "[A-Z]")))
                {
                    if (user.Password.Length > 6)
                    {
                        if (!dbContext.Users.Any(u => u.UserName == user.Username))
                        {
                            if (!dbContext.Users.Any(u => u.Email == user.Email))
                            {
                                return new CommonResult(CommonResultState.OK, "Wszystkie dane są poprawne");
                            }

                            return new CommonResult(CommonResultState.Warning, "Ten login jest zajęty");
                        }

                        return new CommonResult(CommonResultState.Warning, "Ten adres e-mail jest już przypisany do innego konta");
                    }

                    return new CommonResult(CommonResultState.Warning, "Hasło musi zawierać min 6 znakow");
                }

                return new CommonResult(CommonResultState.Warning, "Nazwa powinna zaczynać się od wielkiej litery");

            }

            return new CommonResult(CommonResultState.Error, "Błąd lol");
        }

        public async Task<CommonResult> UpdateUser(UserModel user)
        {
            try
            {
                CommonResult checkResult = CheckUser(user);

                if (checkResult.StateMessage == CommonResultState.OK)
                {
                    var newUser = dbContext.Users.Where(u => u.Id.Equals(user.Id)).FirstOrDefault();

                    newUser.FirstName = user.FirstName;
                    newUser.LastName = user.LastName;
                    newUser.Email = user.Email;
                    newUser.UserName = user.Username;
                    await userManager.AddPasswordAsync(newUser, user.Password);

                    dbContext.Users.Update(newUser);

                    await dbContext.SaveChangesAsync();

                    return new CommonResult(CommonResultState.OK, "Zmiana nazwy powiodła sie");
                }

                return checkResult;
            }
            catch (Exception)
            {
                return new CommonResult(CommonResultState.Error, "Błąd podczas update'owania danych użytkownika");
            }
        }
    }
}
