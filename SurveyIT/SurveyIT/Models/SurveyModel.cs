using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models
{
    public class SurveyModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Start_date { get; set; }

        public DateTime End_date { get; set; }

        public List<string> GroupId { get; set; }

        public List<QuestionModel> Questions { get; set; }
    }
}
