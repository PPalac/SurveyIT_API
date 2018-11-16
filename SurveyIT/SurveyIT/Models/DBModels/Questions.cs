using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models.DBModels
{
    public class Questions
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public List<Questions_List> QuestionsList { get; set; }

        public List<Answers_List> AnswerList { get; set; }
    }
}
