using SurveyIT.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models
{
    public class UserModel : RegistrationModel
    {
        public string Id { get; set; }

        public Role Role { get; set; }
    }
}
