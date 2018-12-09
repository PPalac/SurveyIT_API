using SurveyIT.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Interfaces.Services
{
    public interface IEmailService
    {
        CommonResult SendEmailsGroups(List<string> userId, List<string> groupId);

        CommonResult SendEmailsSurveys(List<string> groupId, List<string> surveyId);
    }
}
