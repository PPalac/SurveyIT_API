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
    public class MyDbContext : IdentityDbContext<Users>
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Answers> Answers { get; set; }
        public DbSet<Answers_List> Answers_List { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<GroupsLink> GroupsLink { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Questions_List> Questions_List { get; set; }
        public DbSet<Surveys> Surveys { get; set; }
        public DbSet<Surveys_List> Surveys_List { get; set; }
        public DbSet<Users> New_Users { get; set; }
    }
}
