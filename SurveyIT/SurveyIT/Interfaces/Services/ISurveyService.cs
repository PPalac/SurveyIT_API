﻿using SurveyIT.Helpers;
using SurveyIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Interfaces.Services
{
    public interface ISurveyService
    {
        CommonResult ValidationSurveyContent(SurveyModel surveyModel);

        Task<CommonResult> AddSurvey(SurveyModel surveyModel);

        Task<CommonResult> AssignSurveysToGroup(List<string> surveyId, List<string> groupId);

        Task<CommonResult> UnAssignSurveysInGroup(List<string> surveyId, List<string> groupId);

        List<SurveyModel> GetAllSurveys();

        SurveyModel GetOneSurvey(string surveyId);

        Task<CommonResult> FillSurvey(string surveyId, List<UserAnswerModel> UserAnswerModelsList, string user);

        CommonResult ValidationFillSurvey(List<UserAnswerModel> UserAnswerModelsList, string surveyId);
    }
}
