using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models
{
    public class UserAnswerModel
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string questionId { get; set; }
    }
}
