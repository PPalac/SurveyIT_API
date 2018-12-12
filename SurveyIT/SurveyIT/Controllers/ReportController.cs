using Microsoft.AspNetCore.Mvc;
using SurveyIT.Attributes;
using SurveyIT.Enums;
using SurveyIT.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Controllers
{
    //[Produces("files")]
    [Route("api/Report")]
    public class ReportController : Controller
    {
        private IReportService reportService;

        public ReportController(IReportService reportService)
        {
            this.reportService = reportService;
        }

        [Auth(Role.Admin)]
        [HttpGet("GetReportXlsx")]
        public async Task<IActionResult> GetReportXlsx([FromQuery]int surveyId)
        {
            string filename = "export.xlsx";
            if (!ModelState.IsValid)
                return BadRequest();

            var memory = reportService.GetReportXlsx(surveyId, filename);

            if (memory != null)
            {
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
                

            return BadRequest();
        }

        [Auth(Role.Admin)]
        [HttpGet("GetReportDocx")]
        public async Task<IActionResult> GetReportDocx([FromQuery]int surveyId)
        {
            string filename = "export.docx";
            if (!ModelState.IsValid)
                return BadRequest();

            var memory = reportService.GetReportDocx(surveyId, filename);

            if (memory != null)
            {
                return File(memory, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", filename);
            }

            return BadRequest();
        }
    }
}
