using Microsoft.AspNetCore.Identity;

namespace SurveyIT.Models.DBModels
{
    public class User : IdentityUser
    { 
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
