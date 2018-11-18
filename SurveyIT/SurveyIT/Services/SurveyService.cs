using SurveyIT.DB;
using SurveyIT.Helpers;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;
using SurveyIT.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SurveyIT.Models.DBModels;

namespace SurveyIT.Services
{
    public class SurveyService : ISurveyService
    {
        private MyDbContext dbContext;

        public SurveyService(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public CommonResult ValidationSurveyContent(SurveyModel surveyModel)
        {
            try
            {
                var resultValidationName = ValidationSurveyName(surveyModel);
                if (resultValidationName.StateMessage == CommonResultState.OK)
                {
                    var resultValidationDate = ValidationSurveyDate(surveyModel);
                    if (resultValidationDate.StateMessage == CommonResultState.OK)
                    {
                        var resultValidationQuestion = ValidationQuestion(surveyModel);
                        if(resultValidationQuestion.StateMessage==CommonResultState.OK)
                        {
                            resultValidationQuestion.Message="Walidacja poprawna";
                            return resultValidationQuestion;
                        }

                        return resultValidationQuestion;
                    }

                    return resultValidationDate;
                }

                return resultValidationName;
            }
            catch (Exception ex)
            {
                return new CommonResult(CommonResultState.Error, "Błąd podczas walidacji");
            }


        }


        public CommonResult ValidationSurveyDate(SurveyModel surveyModel)
        {
            if (surveyModel.Start_date > surveyModel.End_date)
                return new CommonResult(CommonResultState.Warning, "Data rozpoczecia jest wieksza niz data zakonczenia");
            else if (surveyModel.Start_date < DateTime.Now.Date)
                return new CommonResult(CommonResultState.Warning, "Data rozpoczecia musi byc wieksza badz od aktualnej");
            else
                return new CommonResult(CommonResultState.OK, "Poprawna walidacja daty");

        }

        public CommonResult ValidationSurveyName(SurveyModel surveyModel)
        {
            if (surveyModel != null)
            {
                if (Regex.IsMatch(surveyModel.Name.First().ToString(), "[A-Z]"))
                {
                    if (surveyModel.Name.Length > 3)
                    {
                        if (dbContext.Surveys.Any(g => g.Name == surveyModel.Name))
                            return new CommonResult(CommonResultState.Warning, "Ankieta o takiej nazwie juz istnieje");
                        else
                            return new CommonResult(CommonResultState.OK, "Walidacja nazwy poprawna");
                    }

                    return new CommonResult(CommonResultState.Warning, "Za krotka nazwa");
                }

                return new CommonResult(CommonResultState.Warning, "Nazwa powinna zaczynać się od wielkiej litery");
            }

            return new CommonResult(CommonResultState.Error, "Walidacja nazwy niepoprawna");
        }

        public CommonResult ValidationQuestion(SurveyModel surveyModel)
        {
            if (surveyModel.Questions != null)
            {
                foreach (var question in surveyModel.Questions)
                {
                    if(!string.IsNullOrEmpty(question.Content))
                    {
                        if (question.Answers != null)
                        {
                            if ((question.QuestionType == QuestionType.Multiple || question.QuestionType == QuestionType.Single) && question.Answers.Count > 1)
                            {
                                foreach (var answer in question.Answers)
                                {
                                    if(string.IsNullOrEmpty(answer.Content))
                                    {
                                        return new CommonResult(CommonResultState.Warning, "Odpowiedz nie zawiera tekstu");
                                    }
                                }
                            }

                            if (question.QuestionType == QuestionType.Open && !string.IsNullOrEmpty(question.Answers.FirstOrDefault().Content))
                            {
                                return new CommonResult(CommonResultState.Warning, "Odpowiedz powinna byc pusta");
                            }
                            
                            return new CommonResult(CommonResultState.OK, "Walidacja pytan poprawna");
                        }

                        return new CommonResult(CommonResultState.Warning, "Pytanie nie zawiera odpowiedzi");
                    }

                    return new CommonResult(CommonResultState.Warning, "Pytanie nie ma tresci");

                }
 
            }
            return new CommonResult(CommonResultState.Warning, "Ankieta nie zawiera pytan");
        }

        public async Task<CommonResult> AddSurvey(SurveyModel surveyModel)
        {
            try
            {
                var resultValidation = ValidationSurveyContent(surveyModel);

                if (resultValidation.StateMessage == CommonResultState.OK)
                {
                    var newSurvey = new Surveys();
                    newSurvey.End_Date = surveyModel.End_date;
                    newSurvey.Start_Date = surveyModel.Start_date;
                    newSurvey.Name = surveyModel.Name;
                    newSurvey.SurveysList = new List<Surveys_List>();

                    var groups = surveyModel.GroupId;
                    var groupsFromDB = dbContext.Groups.ToList();

                    if (groups != null)
                    {
                        foreach (var group in groups)
                        {
                            if(groupsFromDB.FirstOrDefault(x=>x.Id.ToString()==group)!=null)
                                newSurvey.SurveysList.Add(new Surveys_List { GroupId = int.Parse(group) });
                        }
                    }
                    dbContext.Surveys.Add(newSurvey);

                    if (surveyModel.Questions != null)
                    {
                        foreach (var question in surveyModel.Questions)
                        {
                            var newQuesion = new Questions();
                            newQuesion.Content = question.Content;
                            newQuesion.QuestionType = question.QuestionType;
                            newQuesion.QuestionsList = new List<Questions_List>();

                            newQuesion.QuestionsList.Add(new Questions_List { SurveyId = newSurvey.Id });
                            dbContext.Questions.Add(newQuesion);

                            if (question.Answers != null)
                            {
                                foreach (var answer in question.Answers)
                                {
                                    var newAnswer = new Answers();
                                    newAnswer.Content = answer.Content;
                                    newAnswer.AnswerList = new List<Answers_List>();

                                    newAnswer.AnswerList.Add(new Answers_List { QuestionId = newQuesion.Id });
                                    dbContext.Answers.Add(newAnswer);
                                }
                            }
                        }
                    }

                    await dbContext.SaveChangesAsync();
                    resultValidation.Message = "Pomyslnie stworzono ankiete";
                    return resultValidation;
                }

                return resultValidation;
            }
            catch (Exception ex)
            {
                return new CommonResult(CommonResultState.Error, "Blad podczas tworzenia ankiety");
            }
        }

        public Task<CommonResult> DeleteSurvey(SurveyModel surveyModel)
        {
            throw new NotImplementedException();
        }
    }
}
