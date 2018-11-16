using Microsoft.AspNetCore.Identity;
using SurveyIT.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurveyIT.Models.DBModels
{
    public class Users : IdentityUser
    { 
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BOD { get; set; }

        public Role Role { get; set; } 

        public List<GroupsLink> GroupsLink { get; set; }
    }
}
