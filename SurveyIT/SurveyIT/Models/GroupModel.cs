using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Models
{
    public class GroupModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<string> UserId { get; set; }
    }
}
