using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models.DBModels
{
    public class Surveys_List
    {
        [Key]
        public int Id { get; set; }

        public Groups Group { get; set; }

        public Surveys Survey { get; set; }
    }
}
