using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models.HelperModel
{
    public class HelperFillSurveyModel
    {
        public string surveyId { get; set; }

        public List<UserAnswerModel> UserAnswerModel { get; set; }
    }
}
