using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models.DBModels
{
    public class Groups
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<GroupsLink> GroupsLink { get; set; }

        public List<Surveys_List> SurveysList { get; set; }
    }
}
