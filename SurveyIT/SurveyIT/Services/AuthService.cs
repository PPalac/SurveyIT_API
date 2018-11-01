using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;

namespace SurveyIT.Services
{
    public class AuthService : IAuthService
    {
        private IConfiguration config;

        public AuthService(IConfiguration config)
        {
            this.config = config;
        }

        public UserModel Authenticate(LoginModel login)
        {
            //throw new Exception("Weź mnie zaimplementuje programisto!");

            return new UserModel { Email = "jakis@tam.email", FirstName = "Józek", LastName = "Kowalski", Username = "Józuś" };
        }

        public string Buildtoken(UserModel user)
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
