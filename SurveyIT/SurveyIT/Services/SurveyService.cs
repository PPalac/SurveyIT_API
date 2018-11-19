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

                            var questionExist = dbContext.Questions.FirstOrDefault(x => x.Content == newQuesion.Content);
                            if (questionExist == null)
                                dbContext.Questions.Add(newQuesion);
                            else
                                newQuesion.Id = questionExist.Id;

                            newQuesion.QuestionsList.Add(new Questions_List { SurveyId = newSurvey.Id });

                            if (question.Answers != null)
                            {
                                foreach (var answer in question.Answers)
                                {
                                    var newAnswer = new Answers();
                                    newAnswer.Content = answer.Content;
                                    newAnswer.AnswerList = new List<Answers_List>();

                                    var answersExist = dbContext.Answers.FirstOrDefault(x => x.Content == newAnswer.Content);
                                    if (answersExist == null)
                                        dbContext.Answers.Add(newAnswer);
                                    else
                                        newAnswer.Id = answersExist.Id;

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


        public async Task<CommonResult> AssignSurveysToGroup(List<string> surveyId, List<string> groupId)
        {
            try
            {
                if (surveyId != null || groupId != null)
                {
                    foreach (var survey in surveyId)
                    {
                        var surveyDB = dbContext.Surveys.FirstOrDefault(x => x.Id.ToString() == survey);

                        if (surveyDB != null)
                        {
                            foreach (var group in groupId)
                            {
                                var groupDB = dbContext.Groups.FirstOrDefault(g => g.Id.ToString() == group);

                                if (groupDB != null)
                                {
                                    var surveyList = dbContext.Surveys_List.Where(x => x.Group.Id == groupDB.Id && x.Survey.Id == surveyDB.Id);

                                    if(surveyList==null)
                                        dbContext.Surveys_List.Add(new Surveys_List { Group = groupDB, Survey = surveyDB });
                                }
                                else
                                {
                                    return new CommonResult(Enums.CommonResultState.Warning, "Nie wszystkie grupy istnieja");
                                }
                            }
                        }
                        else
                        {
                            return new CommonResult(Enums.CommonResultState.Warning, "Nie wszystkie ankiety istnieja");
                        }
                    }


                    await dbContext.SaveChangesAsync();
                    return new CommonResult(Enums.CommonResultState.OK, "Przypisanie poprawne");
                }

                return new CommonResult(Enums.CommonResultState.Warning, "Brak obiektow do przypisania");
            }
            catch (Exception ex)
            {
                return new CommonResult(Enums.CommonResultState.Error, "Blad podczas przypisywania");
            }
        }

        public SurveyModel GetOneSurvey(string surveyId)
        {
            try
            {
                if(!string.IsNullOrEmpty(surveyId))
                {
                    var survey = dbContext.Surveys.ToList().FirstOrDefault(s => s.Id.ToString() == surveyId);

                    if(survey!=null)
                    {
                        SurveyModel surveyModel = new SurveyModel();
                        surveyModel.End_date = survey.End_Date;
                        surveyModel.Start_date = survey.Start_Date;
                        surveyModel.Id = survey.Id;
                        surveyModel.Name = survey.Name;
                        surveyModel.Questions = new List<QuestionModel>();

                        var questionLink = dbContext.Questions_List.ToList().Where(q => q.Survey.Id == survey.Id);

                        if(questionLink!=null)
                        {
                            foreach (var qLink in questionLink)
                            {
                                var question = dbContext.Questions.ToList().Where(q => q.Id == qLink.Question.Id);
                                if (question!=null)
                                {
                                    foreach (var oneQuestion in question)
                                    {
                                        QuestionModel questionModel = new QuestionModel();
                                        questionModel.Content = oneQuestion.Content;
                                        questionModel.QuestionType = oneQuestion.QuestionType;
                                        questionModel.Id = oneQuestion.Id;
                                        questionModel.Answers = new List<AnswerModel>();

                                        var answerLink = dbContext.Answers_List.ToList().Where(a => a.Question.Id == oneQuestion.Id);

                                        if(answerLink!=null)
                                        {
                                            foreach (var aLink in answerLink)
                                            {
                                                var answer = dbContext.Answers.ToList().Where(a => a.Id == aLink.Answer.Id);

                                                foreach (var an in answer)
                                                {
                                                    AnswerModel answerModel = new AnswerModel();
                                                    answerModel.Content = an.Content;
                                                    answerModel.Id = an.Id;

                                                    questionModel.Answers.Add(answerModel);
                                                }
                                            }
                                        }

                                        surveyModel.Questions.Add(questionModel);
                                    }
                                }
                            }
                        }
                        else
                        {
                            return null;
                        }
                        return surveyModel;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd wyswietlania");
            }
        }

        public async Task<CommonResult> UnAssignSurveysInGroup(List<string> surveyId, List<string> groupId)
        {
            try
            {
                if (surveyId != null || groupId != null)
                {
                    foreach (var survey in surveyId)
                    {
                        var surveyDB = dbContext.Surveys.FirstOrDefault(x => x.Id.ToString() == survey);
                        if (surveyDB != null)
                        {
                            foreach (var group in groupId)
                            {
                                var groupDB = dbContext.Groups.FirstOrDefault(g => g.Id.ToString() == group);

                                if (groupDB != null)
                                {
                                    var surveyList = dbContext.Surveys_List.Where(x => x.Group.Id == groupDB.Id && x.Survey.Id == surveyDB.Id);

                                    if (surveyList != null)
                                        dbContext.Surveys_List.Remove(new Surveys_List { Group = groupDB, Survey = surveyDB });
                                }
                                else
                                {
                                    return new CommonResult(Enums.CommonResultState.Warning, "Nie wszystkie grupy istnieja");
                                }
                            }
                        }
                        else
                        {
                            return new CommonResult(Enums.CommonResultState.Warning, "Nie wszystkie ankiety istnieja");
                        }
                    }


                    await dbContext.SaveChangesAsync();
                    return new CommonResult(Enums.CommonResultState.OK, "Przypisanie poprawne");
                }

                return new CommonResult(Enums.CommonResultState.Warning, "Brak obiektow do przypisania");
            }
            catch (Exception ex)
            {
                return new CommonResult(Enums.CommonResultState.Error, "Blad podczas przypisywania");
            }
        }

        public SortedList<string, string> GetAllSurveys()
        {
            try
            {
                SortedList<string, string> surveyList = new SortedList<string, string>();
                var surveys = dbContext.Surveys.ToList();

                if (surveys != null)
                {
                    foreach (var survey in surveys)
                    {
                        surveyList.Add(survey.Id.ToString(), survey.Name);
                    }

                    return surveyList;
                }

                return null;
            }
            catch (Exception)
            {
                throw new Exception("Błąd wyswietlania");
            }
        }
    }
}
