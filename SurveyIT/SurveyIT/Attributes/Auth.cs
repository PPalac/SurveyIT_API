using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SurveyIT.Enums;

namespace SurveyIT.Attributes
{
    public class Auth : AuthorizeAttribute
    {
        public Auth(params Role[] roles)
        {
            Roles = string.Join(',', roles.Select(r => r.ToString()));
        }
    }
}
