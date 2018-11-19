using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models.DBModels
{
    public class UserAnswers
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public List<UserAnswers_List> UserAnswerList { get; set; }

        public List<UsersLink> UserLinkL { get; set; }
    }
}
