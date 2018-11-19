﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SurveyIT.DB;
using SurveyIT.Enums;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;
using SurveyIT.Models.DBModels;

namespace SurveyIT.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration config;
        private readonly UserManager<Users> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly MyDbContext dbContext;

        public AuthService(IConfiguration config, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager, MyDbContext dbContext)
        {
            this.config = config;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
        }

        public async Task<bool> RegisterUser(RegistrationModel userData)
        {
            await CreateRoles();

            var user = new Users { FirstName = userData.FirstName, LastName = userData.LastName, Email = userData.Email, UserName = userData.Username };

            var result = await userManager.CreateAsync(user, userData.Password);

            if (!result.Succeeded)
                return false;

            var addToRoleResult = await userManager.AddToRoleAsync(user, Role.User.ToString());

            if (!addToRoleResult.Succeeded)
                return false;

            return true;
        }

        public async Task<Users> Authenticate(LoginModel login)
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

        public ClaimsPrincipal Login(Users user)
        {
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            //var credentials = new SigningCredentials(
            //    key, 
            //    SecurityAlgorithms.HmacSha256);

            //var token = new JwtSecurityToken(
            //    config["Jwt:Issuer"], 
            //    config["Jwt:Issuer"], 
            //    expires: DateTime.Now.AddMinutes(30), 
            //    signingCredentials: credentials);

            //return new JwtSecurityTokenHandler().WriteToken(token);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            return principal;
        }

        private async Task CreateRoles()
        {
            if (!await roleManager.RoleExistsAsync(Role.Admin.ToString()))
                await roleManager.CreateAsync(new IdentityRole { Name = Role.Admin.ToString() });

            if (!await roleManager.RoleExistsAsync(Role.User.ToString()))
                await roleManager.CreateAsync(new IdentityRole { Name = Role.User.ToString() });
        }

    }
}
