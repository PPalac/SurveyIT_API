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

        List<UserModel> GetAllUsersWithRoleUser();

        UserModel GetOneUserById(string userId);

        Task<UserModel> GetUserByUsername(string username);

        List<SurveyModel> GetAllNotFillSurvey(string userId);

        List<SurveyModel> GetAllFillSurvey(string userId);

        List<SurveyModel> GetAllNotFillSurveyAfterDate(string userId);

        SurveyModel GetOneNotFillSurvey(string surveyId);

        SurveyModel GetOneFillSurvey(string userId, string surveyId);

    }
}
