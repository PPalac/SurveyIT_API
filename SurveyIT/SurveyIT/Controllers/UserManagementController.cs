using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SurveyIT.Controllers
{
    public class UserManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}