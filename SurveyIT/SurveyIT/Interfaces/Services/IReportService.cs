using Microsoft.AspNetCore.Mvc;
using SurveyIT.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Interfaces.Services
{
    public interface IReportService
    {
        MemoryStream GetReportXlsx(int id, string fileName);
        MemoryStream GetReportDocx(int id, string fileName);
    }
}
