using SurveyIT.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models.DBModels
{
    public class Answers
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; }

        public List<Answers_List> AnswerList { get; set; }
    }
}
