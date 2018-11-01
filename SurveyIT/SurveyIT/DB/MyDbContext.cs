using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurveyIT.Models.DBModels;

namespace SurveyIT.DB
{
    public class MyDbContext : IdentityDbContext<User>
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Respondent> Respondents { get; set; }
    }
}
