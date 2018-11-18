using SurveyIT.Helpers;
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

        Task<CommonResult> DeleteSurvey(SurveyModel surveyModel);
    }
}
