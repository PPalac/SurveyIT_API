using SurveyIT.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models
{
    public class QuestionModel
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public List<string> SurveyId { get; set; }

        public List<AnswerModel> Answers { get; set; }

        public QuestionType QuestionType { get; set; }
    }
}
