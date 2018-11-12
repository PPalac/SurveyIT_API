using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models.DBModels
{
    public class Questions_List
    {
        [Key]
        public int Id { get; set; }

        public Surveys Survey_ { get; set; }

        public Questions Question_ { get; set; }
    }
}
