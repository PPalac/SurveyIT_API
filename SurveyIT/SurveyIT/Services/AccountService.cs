using Microsoft.EntityFrameworkCore;
using SurveyIT.DB;
using SurveyIT.Enums;
using SurveyIT.Helpers;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;
using SurveyIT.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;


namespace SurveyIT.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<Users> userManager;

        public AccountService(UserManager<Users> userManager)
        {
            this.userManager = userManager;
        }


        private MyDbContext dbContext;

        public AccountService(MyDbContext dbContext)
        {
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

        public List<UserModel> GetAllUsersWithRoleUser()
        {
            try
            {
                List<UserModel> userList = new List<UserModel>();
                var users = dbContext.Users.ToList().Where(x=>x.Role==Role.User);

                if (users != null)
                {
                    foreach (var user in users)
                    {
                        UserModel newUserModel = new UserModel();
                        newUserModel.Username = user.UserName;
                        newUserModel.Id = int.Parse(user.Id);
                        userList.Add(newUserModel);
                    }

                    return userList;
                }

                return null;
            }
            catch (Exception)
            {
                throw new Exception("Błąd wyswietlania");
            }
        }

        public UserModel GetOneUserById(string userId)
        {
            try
            {
                UserModel userModel = new UserModel();
                var user = dbContext.Users.ToList().FirstOrDefault(x => x.Id == userId);

                if(user!=null)
                {
                    userModel.Email = user.Email;
                    userModel.FirstName = user.FirstName;
                    userModel.Id = int.Parse(user.Id);
                    userModel.LastName = user.LastName;
                    userModel.Username = user.UserName;

                    return userModel;
                }

                return null;
            }
            catch (Exception)
            {
                throw new Exception("Błąd wyswietlania");
            }
        }
    }
}
