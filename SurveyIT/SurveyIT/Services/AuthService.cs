using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SurveyIT.DB;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;
using SurveyIT.Models.DBModels;

namespace SurveyIT.Services
{
    public class AuthService : IAuthService
    {
        private IConfiguration config;
        private UserManager<User> userManager;
        private MyDbContext dbContext;

        public AuthService(IConfiguration config, UserManager<User> userManager, MyDbContext dbContext)
        {
            this.config = config;
            this.userManager = userManager;
            this.dbContext = dbContext;
        }
        public async Task<bool> RegisterUser(RegistrationModel userData)
        {
            var user = new User { FirstName = userData.FirstName, LastName = userData.LastName, Email = userData.Email, UserName = userData.Username };

            var result = await userManager.CreateAsync(user, userData.Password);

            if (!result.Succeeded)
                return false;

            await dbContext.Respondents.AddAsync(new Respondent { IdentityId = user.Id, GroupId = 0 });
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<User> Authenticate(LoginModel login)
        {
            //throw new Exception("Weź mnie zaimplementuje programisto!");

            var user = await userManager.FindByNameAsync(login.Username);

            if (user != null)
            {
                if (await userManager.CheckPasswordAsync(user, login.Password))
                {
                    return user;
                }
            }

            return null;
        }

        public string Buildtoken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(
                key, 
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                config["Jwt:Issuer"], 
                config["Jwt:Issuer"], 
                expires: DateTime.Now.AddMinutes(30), 
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
