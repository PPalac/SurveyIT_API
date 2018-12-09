﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SurveyIT.DB;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models.DBModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Services
{
    public class ReportService : IReportService
    {
        private IHostingEnvironment hostingEnvironment;
        private MyDbContext dbContext;

        public ReportService(IHostingEnvironment hostingEnvironment, MyDbContext dbContext)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.dbContext = dbContext;
        }

        MemoryStream IReportService.GetReportXlsx(int id, string fileName)
        {
            Surveys survey = dbContext.Surveys.FirstOrDefault(x => x.Id == id);
            if (survey != null)
            {
                // Get survey questions
                List<Questions> questions = survey.QuestionsList.Where(x => x.Survey.Id == id).ToList().Select(p => p.Question).ToList();

                // Lets make file
                string sWebRootFolder = hostingEnvironment.WebRootPath;
                string sFileName = fileName;
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                var memory = new MemoryStream();
                int line = 0;
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Raport");

                    // Create header
                    IRow row = excelSheet.CreateRow(line++);
                    row.CreateCell(0).SetCellValue("Raport");
                    row.CreateCell(1).SetCellValue(survey.Name);
                    // One line for space
                    excelSheet.CreateRow(line++);

                    foreach (Questions question in questions)
                    {
                        switch(question.QuestionType)
                        {
                            case Enums.QuestionType.Single:
                                {
                                    // Print question
                                    row = excelSheet.CreateRow(line++);
                                    row.CreateCell(0).SetCellValue("Pytanie jednokrotnego wyboru");
                                    row.CreateCell(1).SetCellValue(question.Content);
                                    row = excelSheet.CreateRow(line++);
                                    row.CreateCell(0).SetCellValue("Odpowiedzi");
                                    // Get question answers
                                    List<Answers> answers = question.AnswerList.ToList().Select(p => p.Answer).ToList();
                                    // For every answer
                                    foreach (Answers answer in answers)
                                    {
                                        row = excelSheet.CreateRow(line++);
                                        // Get answer
                                        row.CreateCell(0).SetCellValue(answer.Content);
                                        // Get count of answers
                                        List<UserAnswers> userAnswers = question.UserAnswerList.ToList().Select(p => p.UserAnswer).Where(q => q.Content == answer.Content).ToList();
                                        // Print count of answers
                                        row.CreateCell(1).SetCellValue(userAnswers.Count);
                                    }
                                    break;
                                }
                            case Enums.QuestionType.Multiple:
                                {
                                    // Print question
                                    row = excelSheet.CreateRow(line++);
                                    row.CreateCell(0).SetCellValue("Pytanie wielokrotnego wyboru");
                                    row.CreateCell(1).SetCellValue(question.Content);
                                    row = excelSheet.CreateRow(line++);
                                    row.CreateCell(0).SetCellValue("Odpowiedzi");
                                    // Get question answers
                                    List<Answers> answers = question.AnswerList.ToList().Select(p => p.Answer).ToList();
                                    // For every answer
                                    foreach (Answers answer in answers)
                                    {
                                        row = excelSheet.CreateRow(line++);
                                        // Get answer
                                        row.CreateCell(0).SetCellValue(answer.Content);
                                        // Get count of answers
                                        List<UserAnswers> userAnswers = question.UserAnswerList.ToList().Select(p => p.UserAnswer).Where(q => q.Content == answer.Content).ToList();
                                        // Print count of answers
                                        row.CreateCell(1).SetCellValue(userAnswers.Count);
                                    }
                                    break;
                                }
                            case Enums.QuestionType.Open:
                                {
                                    // Print question
                                    row = excelSheet.CreateRow(line++);
                                    row.CreateCell(0).SetCellValue("Pytanie otwarte");
                                    row.CreateCell(1).SetCellValue(question.Content);
                                    row = excelSheet.CreateRow(line++);
                                    row.CreateCell(0).SetCellValue("Odpowiedzi");
                                    // Get user answers
                                    List<UserAnswers> userAnswers = question.UserAnswerList.ToList().Select(p => p.UserAnswer).ToList(); ;
                                    // For every userAnswer
                                    foreach (UserAnswers userAnswer in userAnswers)
                                    {
                                        row = excelSheet.CreateRow(line++);
                                        // Print answer
                                        row.CreateCell(0).SetCellValue(userAnswer.Content);
                                    }
                                    break;
                                }
                        }
                    }
                    // Save spreadsheet
                    workbook.Write(fs);

                }
                // Prepare file to return
                using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;
                return memory;
            }
            else
            {
                // "Survey doesn't exist"
                return null;        
            }
        }
    }
}