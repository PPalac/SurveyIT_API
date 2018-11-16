using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models.DBModels
{
    public class Answers_List
    {
        [Key]
        public int Id { get; set; }

        public Questions Question { get; set; }

        public Answers Answer { get; set; }
    }
}
