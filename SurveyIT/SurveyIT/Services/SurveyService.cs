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
        private EmailService emailService;

        public SurveyService(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
            emailService = new EmailService(dbContext);
        }

        #region ValidationRegion
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
                        if (resultValidationQuestion.StateMessage == CommonResultState.OK)
                        {
                            resultValidationQuestion.Message = Properties.Resources.CorrectValidation;
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
                return new CommonResult(CommonResultState.Error, Properties.Resources.ErrorValidation);
            }


        }


        public CommonResult ValidationSurveyDate(SurveyModel surveyModel)
        {
            if (surveyModel.Start_date > surveyModel.End_date)
                return new CommonResult(CommonResultState.Warning, Properties.Resources.NotValidData1);
            else if (surveyModel.Start_date < DateTime.Now.Date)
                return new CommonResult(CommonResultState.Warning, Properties.Resources.NotValidData2);
            else
                return new CommonResult(CommonResultState.OK, Properties.Resources.CorrectDataValidation);

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
                            return new CommonResult(CommonResultState.Warning, Properties.Resources.SurveyNameUsed);
                        else
                            return new CommonResult(CommonResultState.OK, Properties.Resources.CorrectValidation);
                    }

                    return new CommonResult(CommonResultState.Warning, Properties.Resources.ShortGroupName);
                }

                return new CommonResult(CommonResultState.Warning, Properties.Resources.NotValidName);
            }

            return new CommonResult(CommonResultState.Error, Properties.Resources.ErrorValidation);
        }

        public CommonResult ValidationQuestion(SurveyModel surveyModel)
        {
            if (surveyModel.Questions != null)
            {
                foreach (var question in surveyModel.Questions)
                {
                    if (!string.IsNullOrEmpty(question.Content))
                    {
                        if (question.Answers != null)
                        {
                            if ((question.QuestionType == QuestionType.Multiple || question.QuestionType == QuestionType.Single) && question.Answers.Count > 1)
                            {
                                foreach (var answer in question.Answers)
                                {
                                    if (string.IsNullOrEmpty(answer.Content))
                                    {
                                        return new CommonResult(CommonResultState.Warning, Properties.Resources.AnswerWithoutContent);
                                    }
                                }
                            }

                            if (question.QuestionType == QuestionType.Open && question.Answers.Count!=0)
                            {
                                return new CommonResult(CommonResultState.Warning, Properties.Resources.Answer1);
                            }
                        }
                        else
                            return new CommonResult(CommonResultState.Warning, Properties.Resources.Question1);
                    }
                    else
                        return new CommonResult(CommonResultState.Warning, Properties.Resources.Question2);

                }

                return new CommonResult(CommonResultState.OK, Properties.Resources.CorrectValidation);
            }
            else
                return new CommonResult(CommonResultState.Warning, Properties.Resources.SurveyWithoutQuestions);
        }

        public CommonResult ValidationFillQuestion(SurveyModel surveyModel)
        {
            if (surveyModel.Questions != null)
            {
                foreach (var question in surveyModel.Questions)
                {
                    if (!string.IsNullOrEmpty(question.Content))
                    {
                        if (question.Answers != null)
                        {
                            if (question.QuestionType == QuestionType.Single)
                            {
                                if (!(question.Answers.Count == 1 && !string.IsNullOrEmpty(question.Answers[0].Content)))
                                    return new CommonResult(CommonResultState.Warning, Properties.Resources.ErrorValidationAnswers);
                            }
                            else if (question.QuestionType == QuestionType.Open)
                            {
                                if (!(question.Answers.Count == 1 && !string.IsNullOrEmpty(question.Answers[0].Content)))
                                {
                                    return new CommonResult(CommonResultState.Warning, Properties.Resources.ErrorValidationAnswers);
                                }
                            }
                            else
                            {
                                if (!(question.Answers.Count > 1))
                                {
                                    foreach (var answer in question.Answers)
                                    {
                                        if (!string.IsNullOrEmpty(answer.Content))
                                        {
                                            return new CommonResult(CommonResultState.Warning, Properties.Resources.ErrorValidationAnswers);
                                        }
                                    }

                                }
                            }
                        }
                        else
                            return new CommonResult(CommonResultState.Warning, Properties.Resources.Question1);
                    }
                    else
                        return new CommonResult(CommonResultState.Warning, Properties.Resources.Question2);

                }

                return new CommonResult(CommonResultState.OK, Properties.Resources.CorrectValidation);

            }
            else
                return new CommonResult(CommonResultState.Warning, Properties.Resources.SurveyWithoutQuestions);
        }

        public CommonResult ValidationFillSurvey(List<UserAnswerModel> UserAnswerModelsList, string surveyId)
        {
            try
            {
                if (!string.IsNullOrEmpty(surveyId))
                {
                    var survey = dbContext.Surveys.FirstOrDefault(x => x.Id.ToString() == surveyId);
                    if (survey != null)
                    {
                        if (UserAnswerModelsList != null)
                        {
                            foreach (var answerModel in UserAnswerModelsList)
                            {
                                if (!string.IsNullOrEmpty(answerModel.questionId))
                                {
                                    if (string.IsNullOrEmpty(answerModel.Content))
                                    {
                                        return new CommonResult(CommonResultState.Warning, Properties.Resources.AnswerIsEmpty);
                                    }
                                }
                                else
                                {
                                    return new CommonResult(CommonResultState.Warning, Properties.Resources.AnswerWithoutQuestio);
                                }
                            }

                            return new CommonResult(CommonResultState.OK, Properties.Resources.CorrectValidation);
                        }

                        return new CommonResult(CommonResultState.Warning, Properties.Resources.AnswerIsEmpty);
                    }
                }

                return new CommonResult(CommonResultState.Warning, Properties.Resources.SurveyNotExist);
            }
            catch (Exception)
            {
                return new CommonResult(CommonResultState.Error, Properties.Resources.ErrorValidation);
            }
        }
        #endregion

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
                            if (groupsFromDB.FirstOrDefault(x => x.Id.ToString() == group) != null)
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
                            dbContext.Questions.Add(newQuesion);

                            newQuesion.QuestionsList.Add(new Questions_List { SurveyId = newSurvey.Id });

                            if (question.Answers != null)
                            {
                                foreach (var answer in question.Answers)
                                {
                                    var newAnswer = new Answers();
                                    newAnswer.Content = answer.Content;
                                    newAnswer.AnswerList = new List<Answers_List>();
                                    dbContext.Answers.Add(newAnswer);

                                    newAnswer.AnswerList.Add(new Answers_List { QuestionId = newQuesion.Id });
                                    dbContext.Answers.Add(newAnswer);
                                }
                            }
                        }
                    }

                    await dbContext.SaveChangesAsync();
                    resultValidation.Message = Properties.Resources.CreateSurvey;
                    return resultValidation;
                }

                return resultValidation;
            }
            catch (Exception ex)
            {
                return new CommonResult(CommonResultState.Error, Properties.Resources.Error);
            }
        }

        #region AssignRegion
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
                                    var surveyList = dbContext.Surveys_List.Where(x => x.Group.Id == groupDB.Id && x.Survey.Id == surveyDB.Id).ToList();

                                    if (surveyList.Count == 0)
                                        dbContext.Surveys_List.Add(new Surveys_List { Group = groupDB, Survey = surveyDB });
                                }
                                else
                                {
                                    return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.GroupNotExist);
                                }
                            }
                        }
                        else
                        {
                            return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.SurveyNotExist);
                        }
                    }

                    emailService.SendEmailsSurveys(groupId, surveyId);
                    await dbContext.SaveChangesAsync();
                    return new CommonResult(Enums.CommonResultState.OK, Properties.Resources.AssignCorrect);
                }

                return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.NoDataToAssign);
            }
            catch (Exception ex)
            {
                return new CommonResult(Enums.CommonResultState.Error, Properties.Resources.Error);
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
                                    var surveyList = dbContext.Surveys_List.FirstOrDefault(x => x.Group.Id == groupDB.Id && x.Survey.Id == surveyDB.Id);

                                    if (surveyList != null)
                                    {
                                        dbContext.Surveys.FirstOrDefault(s => s.Id == int.Parse(survey)).SurveysList.Remove(surveyList);
                                        dbContext.Groups.FirstOrDefault(g => g.Id == int.Parse(group)).SurveysList.Remove(surveyList);
                                        dbContext.Surveys_List.Remove(surveyList);
                                    }

                                }
                                else
                                {
                                    return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.GroupNotExist);
                                }
                            }
                        }
                        else
                        {
                            return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.SurveyNotExist);
                        }
                    }


                    await dbContext.SaveChangesAsync();
                    return new CommonResult(Enums.CommonResultState.OK, Properties.Resources.AssignCorrect);
                }

                return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.NoDataToAssign);
            }
            catch (Exception ex)
            {
                return new CommonResult(Enums.CommonResultState.Error, Properties.Resources.Error);
            }
        }
        #endregion


        public SurveyModel GetOneSurvey(string surveyId)
        {
            try
            {
                if (!string.IsNullOrEmpty(surveyId))
                {
                    var survey = dbContext.Surveys.ToList().FirstOrDefault(s => s.Id.ToString() == surveyId);

                    if (survey != null)
                    {
                        SurveyModel surveyModel = new SurveyModel();
                        surveyModel.End_date = survey.End_Date;
                        surveyModel.Start_date = survey.Start_Date;
                        surveyModel.Id = survey.Id;
                        surveyModel.Name = survey.Name;
                        surveyModel.Questions = new List<QuestionModel>();

                        var questionLink = dbContext.Questions_List.ToList().Where(q => q.Survey.Id == survey.Id);

                        if (questionLink != null)
                        {
                            foreach (var qLink in questionLink)
                            {
                                var question = dbContext.Questions.ToList().Where(q => q.Id == qLink.Question.Id);
                                if (question != null)
                                {
                                    foreach (var oneQuestion in question)
                                    {
                                        QuestionModel questionModel = new QuestionModel();
                                        questionModel.Content = oneQuestion.Content;
                                        questionModel.QuestionType = oneQuestion.QuestionType;
                                        questionModel.Id = oneQuestion.Id;
                                        questionModel.Answers = new List<AnswerModel>();

                                        var answerLink = dbContext.Answers_List.ToList().Where(a => a.Question.Id == oneQuestion.Id);

                                        if (answerLink != null)
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
                throw new Exception(Properties.Resources.ErrorDisplay);
            }
        }


        public List<Tuple<string, string>> GetAllSurveys()
        {
            try
            {
                List<Tuple<string, string>> surveyList = new List<Tuple<string, string>>();
                var surveys = dbContext.Surveys.ToList();

                if (surveys != null)
                {
                    foreach (var survey in surveys)
                    {
                        surveyList.Add(new Tuple<string, string>(survey.Id.ToString(), survey.Name));
                    }

                    return surveyList;
                }

                return null;
            }
            catch (Exception)
            {
                throw new Exception(Properties.Resources.ErrorDisplay);
            }
        }

        #region FillSurvey
        public async Task<CommonResult> FillSurvey(string surveyId, List<UserAnswerModel> UserAnswerModelsList, string user)
        {
            try
            {
                var validationFillSurvey = ValidationFillSurvey(UserAnswerModelsList, surveyId);

                if (validationFillSurvey.StateMessage == CommonResultState.OK)
                {
                    foreach (var userAnswer in UserAnswerModelsList)
                    {
                        UserAnswers usAnswer = new UserAnswers();
                        usAnswer.Content = userAnswer.Content;
                        usAnswer.UserAnswerList = new List<UserAnswers_List>();
                        usAnswer.UserLinkL = new List<UsersLink>();

                        usAnswer.UserAnswerList.Add(new UserAnswers_List { QuestionId = int.Parse(userAnswer.questionId) });

                        var userId = dbContext.Users.FirstOrDefault(x => x.Id == user);

                        if (userId != null)
                            usAnswer.UserLinkL.Add(new UsersLink { UserId = userId.Id });
                        else
                            return new CommonResult(CommonResultState.Error, Properties.Resources.UserNotExist);

                        dbContext.UserAnswers.Add(usAnswer);

                    }


                    await dbContext.SaveChangesAsync();
                    validationFillSurvey.Message = Properties.Resources.FillSurvey;
                    return validationFillSurvey;

                }

                return validationFillSurvey;
            }
            catch (Exception)
            {
                return new CommonResult(CommonResultState.Error, Properties.Resources.Error);
            }
        }
        #endregion
    }
}
