using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurveyIT.DB;
using SurveyIT.Enums;
using SurveyIT.Helpers;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;
using SurveyIT.Models.DBModels;


namespace SurveyIT.Services
{
    public class AccountService : IAccountService
    {
        private UserManager<Users> userManager;
        private MyDbContext dbContext;

        public AccountService(UserManager<Users> userManager, MyDbContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        private CommonResult CheckUser(UserModel user)
        {
            if (user != null)
            {
                if ((Regex.IsMatch(user.FirstName.First().ToString(), "[A-Z]")) && (Regex.IsMatch(user.LastName.First().ToString(), "[A-Z]")))
                {
                    if (string.IsNullOrEmpty(user.Password) || user.Password.Length > 6)
                    {
                        if (!dbContext.Users.Any(u => u.UserName == user.Username && u.Id != user.Id))
                        {
                            if (!dbContext.Users.Any(u => u.Email == user.Email && u.UserName != user.Username))
                            {
                                return new CommonResult(CommonResultState.OK, Properties.Resources.DataCorrect);
                            }

                            return new CommonResult(CommonResultState.Warning, Properties.Resources.LoginIsUsed);
                        }

                        return new CommonResult(CommonResultState.Warning, Properties.Resources.EmailIsUsed);
                    }

                    return new CommonResult(CommonResultState.Warning, Properties.Resources.ShortPass);
                }

                return new CommonResult(CommonResultState.Warning, Properties.Resources.NotValidName);

            }

            return new CommonResult(CommonResultState.Error, Properties.Resources.Error);
        }

        public async Task<CommonResult> UpdateUser(UserModel user)
        {
            try
            {
                CommonResult checkResult = CheckUser(user);

                if (checkResult.StateMessage == CommonResultState.OK)
                {
                    var newUser = dbContext.Users.Where(u => u.Id.Equals(user.Id)).FirstOrDefault();

                    newUser.FirstName = user.FirstName;
                    newUser.LastName = user.LastName;
                    newUser.Email = user.Email;
                    newUser.UserName = user.Username;

                    if (!string.IsNullOrEmpty(user.Password))
                        await userManager.AddPasswordAsync(newUser, user.Password);

                    dbContext.Users.Update(newUser);

                    await dbContext.SaveChangesAsync();

                    return new CommonResult(CommonResultState.OK, Properties.Resources.ChangedName);
                }

                return checkResult;
            }
            catch (Exception)
            {
                return new CommonResult(CommonResultState.Error, Properties.Resources.UpdateNameError);
            }
        }

        public List<UserModel> GetAllUsersWithRoleUser()
        {
            try
            {
                List<UserModel> userList = new List<UserModel>();
                var users = dbContext.Users.ToList().Where(x=>x.Role==Role.User);

                if (users != null)
                {
                    foreach (var user in users)
                    {
                        UserModel newUserModel = new UserModel();
                        newUserModel.Username = user.UserName;
                        newUserModel.Id = user.Id;
                        userList.Add(newUserModel);
                    }

                    return userList;
                }

                return null;
            }
            catch (Exception)
            {
                throw new Exception(Properties.Resources.ErrorDisplay);
            }
        }

        public UserModel GetOneUserById(string userId)
        {
            try
            {
                UserModel userModel = new UserModel();
                var user = dbContext.Users.ToList().FirstOrDefault(x => x.Id == userId);

                if(user!=null)
                {
                    userModel.Email = user.Email;
                    userModel.FirstName = user.FirstName;
                    userModel.Id = user.Id;
                    userModel.LastName = user.LastName;
                    userModel.Username = user.UserName;
                    userModel.Role = user.Role;

                    return userModel;
                }

                return null;
            }
            catch (Exception)
            {
                throw new Exception(Properties.Resources.ErrorDisplay);
            }
        }

        public async Task<UserModel> GetUserByUsername(string username)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user != null)
            {
                return new UserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.UserName
                };
            }

            return null;
        }

        public List<SurveyModel> GetAllNotFillSurvey(string userId)
        {
            try
            {
                if(!string.IsNullOrEmpty(userId))
                {
                    var user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
                    List<SurveyModel> surveyModelList = new List<SurveyModel>();

                    if(user!=null)
                    {
                        var AllGroups = dbContext
                            .Groups
                            .Include(g => g.GroupsLink)
                            .Include(g => g.SurveysList)
                            .Where(g => g.GroupsLink.Any(gl => gl.UserId == userId)).Select(g=>g.Id.ToString());

                        var surveyId = dbContext
                            .Surveys
                            .Include(s => s.SurveysList)
                                .ThenInclude(sl => sl.Group)
                            .Where(s => s.SurveysList.Any(sl => AllGroups.Contains(sl.Group.Id.ToString())) && s.End_Date>DateTime.Now).Select(s=>s.Id.ToString()).ToList();

                        var userAnswers = dbContext
                            .UserAnswers
                            .Include(a => a.UserLinkL)
                                .ThenInclude(al => al.User)
                            .Where(a => a.UserLinkL.Any(al => al.UserId == userId)).Select(a=>a.Id).ToList();

                        var questionId = dbContext
                            .Questions
                            .Include(q => q.UserAnswerList)
                                .ThenInclude(al => al.UserAnswer)
                            .Where(q => q.UserAnswerList.Any(al => userAnswers.Contains(al.UserAnswer.Id))).Select(q => q.Id).ToList();

                        var surveyFillId = dbContext
                            .Questions_List
                            .Include(ql => ql.Question)
                            .Where(ql => questionId.Contains(ql.Question.Id)).Select(ql => ql.Survey.Id.ToString()).ToList();

                        surveyFillId = surveyFillId.Distinct().ToList();
                        surveyId = surveyId.Except(surveyFillId).ToList();

                        if (surveyId != null)
                        {
                            foreach (var survey in surveyId)
                            {
                                var surveyDb = dbContext.Surveys.FirstOrDefault(s => s.Id.ToString() == survey);

                                if (surveyDb != null)
                                {
                                    surveyModelList.Add(new SurveyModel
                                    {
                                        End_date = surveyDb.End_Date,
                                        Name = surveyDb.Name,
                                        Start_date = surveyDb.Start_Date,
                                        Id = surveyDb.Id
                                    });
                                }
                            }
                        }

                        return surveyModelList;
                        
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<SurveyModel> GetAllFillSurvey(string userId)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
                    List<SurveyModel> surveyModelList = new List<SurveyModel>();

                    if (user != null)
                    {
                        var userAnswers = dbContext
                            .UserAnswers
                            .Include(a => a.UserLinkL)
                                .ThenInclude(al => al.User)
                            .Where(a => a.UserLinkL.Any(al => al.UserId == userId)).Select(a => a.Id).ToList();

                        var questionId = dbContext
                            .Questions
                            .Include(q => q.UserAnswerList)
                                .ThenInclude(al => al.UserAnswer)
                            .Where(q => q.UserAnswerList.Any(al => userAnswers.Contains(al.UserAnswer.Id))).Select(q => q.Id).ToList();

                        var surveyFillId = dbContext
                            .Questions_List
                            .Include(ql => ql.Question)
                            .Where(ql => questionId.Contains(ql.Question.Id)).Select(ql => ql.Survey.Id.ToString()).ToList();

                        surveyFillId = surveyFillId.Distinct().ToList();

                        if (surveyFillId != null)
                        {
                            foreach (var survey in surveyFillId)
                            {
                                var surveyDb = dbContext.Surveys.FirstOrDefault(s => s.Id.ToString() == survey);

                                if (surveyDb != null)
                                {
                                    surveyModelList.Add(new SurveyModel
                                    {
                                        End_date = surveyDb.End_Date,
                                        Name = surveyDb.Name,
                                        Start_date = surveyDb.Start_Date,
                                        Id = surveyDb.Id
                                    });
                                }
                            }
                        }

                        return surveyModelList;

                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<SurveyModel> GetAllNotFillSurveyAfterDate(string userId)
        {
            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
                    List<SurveyModel> surveyModelList = new List<SurveyModel>();

                    if (user != null)
                    {
                        var AllGroups = dbContext
                            .Groups
                            .Include(g => g.GroupsLink)
                            .Include(g => g.SurveysList)
                            .Where(g => g.GroupsLink.Any(gl => gl.UserId == userId)).Select(g => g.Id.ToString());

                        var surveyId = dbContext
                            .Surveys
                            .Include(s => s.SurveysList)
                                .ThenInclude(sl => sl.Group)
                            .Where(s => s.SurveysList.Any(sl => AllGroups.Contains(sl.Group.Id.ToString())) && s.End_Date < DateTime.Now).Select(s => s.Id.ToString()).ToList();

                        var userAnswers = dbContext
                            .UserAnswers
                            .Include(a => a.UserLinkL)
                                .ThenInclude(al => al.User)
                            .Where(a => a.UserLinkL.Any(al => al.UserId == userId)).Select(a => a.Id).ToList();

                        var questionId = dbContext
                            .Questions
                            .Include(q => q.UserAnswerList)
                                .ThenInclude(al => al.UserAnswer)
                            .Where(q => q.UserAnswerList.Any(al => userAnswers.Contains(al.UserAnswer.Id))).Select(q => q.Id).ToList();

                        var surveyFillId = dbContext
                            .Questions_List
                            .Include(ql => ql.Question)
                            .Where(ql => questionId.Contains(ql.Question.Id)).Select(ql => ql.Survey.Id.ToString()).ToList();

                        surveyFillId = surveyFillId.Distinct().ToList();
                        surveyId = surveyId.Except(surveyFillId).ToList();

                        if (surveyId != null)
                        {
                            foreach (var survey in surveyId)
                            {
                                var surveyDb = dbContext.Surveys.FirstOrDefault(s => s.Id.ToString() == survey);

                                if (surveyDb != null)
                                {
                                    surveyModelList.Add(new SurveyModel
                                    {
                                        End_date = surveyDb.End_Date,
                                        Name = surveyDb.Name,
                                        Start_date = surveyDb.Start_Date,
                                        Id = surveyDb.Id
                                    });
                                }
                            }
                        }

                        return surveyModelList;

                    }
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SurveyModel GetOneNotFillSurvey(string surveyId)
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

        public SurveyModel GetOneFillSurvey(string userId, string surveyId)
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
                                        questionModel.UserAnswers = new List<UserAnswerModel>();

                                        var allUserList = dbContext.UsersLink.Where(usl => usl.UserId == userId).Select(usl => usl.Id.ToString()).ToList();

                                        var allUserAnswerList = dbContext.UserAnswers_Lists.Where(ual => ual.QuestionId == oneQuestion.Id).Select(ual=>ual.Id.ToString()).ToList();

                                        var allAnswer = allUserAnswerList.Intersect(allUserList).ToList();

                                        if(allAnswer != null)
                                        {
                                            

                                            foreach (var answer in allAnswer)
                                            {
                                                var usDb = dbContext.UserAnswers.ToList().Where(us => us.Id == int.Parse(answer));

                                                foreach (var us in usDb)
                                                {
                                                    UserAnswerModel userAnswerModel = new UserAnswerModel();
                                                    userAnswerModel.Content = us.Content;
                                                    userAnswerModel.questionId = oneQuestion.Id.ToString();
                                                    userAnswerModel.Id = us.Id;

                                                    questionModel.UserAnswers.Add(userAnswerModel);
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
    }
}
