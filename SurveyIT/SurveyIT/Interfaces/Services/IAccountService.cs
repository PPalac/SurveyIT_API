using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyIT.Helpers;
using SurveyIT.Models;
using SurveyIT.Enums;


namespace SurveyIT.Interfaces.Services
{
    public interface IAccountService
    {
        Task<CommonResult> UpdateUser(UserModel user);

    }
}
