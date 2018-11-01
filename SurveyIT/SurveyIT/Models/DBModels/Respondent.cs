using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurveyIT.Enums;

namespace SurveyIT.Models.DBModels
{
    public class Respondent
    {
        public int Id { get; set; }

        public string IdentityId { get; set; }

        public User User { get; set; }

        public int GroupId { get; set; }
    }
}
