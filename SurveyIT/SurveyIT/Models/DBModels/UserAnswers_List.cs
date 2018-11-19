using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models.DBModels
{
    public class UserAnswers_List
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        public Questions Question { get; set; }

        public UserAnswers UserAnswer { get; set; }
    }
}
