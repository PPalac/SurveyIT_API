using SurveyIT.DB;
using SurveyIT.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using SurveyIT.Helpers;
using Microsoft.EntityFrameworkCore;

namespace SurveyIT.Services
{
    public class EmailService : IEmailService
    {
        private MyDbContext dbContext;
        private Properties.Resources resources;

        public EmailService(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public CommonResult SendEmailsGroups(List<string> userId, List<string> groupId)
        {
            try
            {
                if (userId != null && groupId != null)
                {
                    var emails = GetEmailsGroups(userId, groupId);

                    if (emails.Count != 0)
                    {
                        using (SmtpClient client = new SmtpClient())
                        {
                            var credentrial = new NetworkCredential
                            {
                                UserName = "projectmail123456test@gmail.com",
                                Password = "TestforTest55"
                            };

                            client.Credentials = credentrial;

                            client.Host = "smtp.gmail.com";
                            client.Port = 587;
                            client.EnableSsl = true;

                            var message = new MailMessage();

                            foreach (var email in emails)
                            {
                                message.To.Add(new MailAddress(email.ToString()));
                            }

                            message.From = new MailAddress("projectmail123456test@gmail.com");
                            message.Subject = "Nowe przypisanie do grupy";
                            message.Body = "Zostałeś/aś przypisany do nowych grup.\n Sprawdź czy nie masz nowych ankiet do uzupełnienia." +
                                "\nPozdrawia\nZespół SurveyIT";

                            client.Send(message);

                            return new CommonResult(Enums.CommonResultState.OK, Properties.Resources.Send);
                        }
                    }

                    return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.NoMail);
                }

                return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.NoDataExist);
            }
            catch (Exception ex)
            {
                return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.ErrorSend);
            }
        }

        public CommonResult SendEmailsSurveys(List<string> groupId, List<string> surveyId)
        {
            try
            {
                if (surveyId != null && groupId != null)
                {
                    var emails = GetEmailsGroups(groupId, surveyId);

                    if (emails.Count != 0)
                    {
                        using (SmtpClient client = new SmtpClient())
                        {
                            var credentrial = new NetworkCredential
                            {
                                UserName = "projectmail123456test@gmail.com",
                                Password = "TestforTest55"
                            };

                            client.Credentials = credentrial;

                            client.Host = "smtp.gmail.com";
                            client.Port = 587;
                            client.EnableSsl = true;

                            var message = new MailMessage();

                            foreach (var email in emails)
                            {
                                message.To.Add(new MailAddress(email.ToString()));
                            }

                            message.From = new MailAddress("projectmail123456test@gmail.com");
                            message.Subject = "Nowe przypisanie do grupy";
                            message.Body = "Zostałeś/aś przypisany do nowych grup.\n Sprawdź czy nie masz nowych ankiet do uzupełnienia." +
                                "\nPozdrawia\nZespół SurveyIT";

                            client.Send(message);

                            return new CommonResult(Enums.CommonResultState.OK, Properties.Resources.Send);
                        }
                    }

                    return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.NoMail);
                }

                return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.NoDataExist);
            }
            catch (Exception)
            {
                return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.ErrorSend);
            }
        }

        private List<string> GetEmailsGroups(List<string> userId, List<string> groupId)
        {
            try
            {
                List<string> emailsList = new List<string>();

                foreach (var groups in groupId)
                {
                    var group = dbContext.Groups.FirstOrDefault(x => x.Id.ToString() == groups);

                    if (group != null)
                    {

                        foreach (var users in userId)
                        {
                            var user = dbContext.Users.FirstOrDefault(u => u.Id == users && u.Role == Enums.Role.User);

                            if (user != null)
                            {
                                var groupLink = dbContext.GroupsLink.Where(x => x.User.Id == user.Id && x.Group.Id == group.Id).ToList();

                                if (groupLink.Count == 0)
                                {
                                    emailsList.Add(user.Email);
                                    break;
                                }
                                //else
                                //    return new CommonResult(Enums.CommonResultState.Warning, "Takie przypisanie juz istnieje");
                            }
                            else
                            {
                                return null;
                            }
                        }


                    }
                    else
                    {
                        return null;
                    }
                }

                return emailsList;

                //foreach (var user in userId)
                //{
                //    var userInDb = dbContext.Users.FirstOrDefault(u => u.Id == user);

                //    foreach (var group in groupId)
                //    {
                //        var validSurveys = dbContext
                //            .Surveys
                //            .Include(s => s.SurveysList)
                //            .Where(s => s.SurveysList.Any(sl => sl.GroupId.ToString() == group) && s.End_Date > DateTime.Now).Select(s => s.Id.ToString()).ToList();

                //        var AllGroups = dbContext
                //            .Groups
                //            .Include(g => g.GroupsLink)
                //            .Include(g => g.SurveysList)
                //            .Where(g => g.GroupsLink.Any(gl => gl.UserId == user)).Select(g => g.Id.ToString());

                //        var surveyId = dbContext
                //            .Surveys
                //            .Include(s => s.SurveysList)
                //                .ThenInclude(sl => sl.Group)
                //            .Where(s => s.SurveysList.Any(sl => AllGroups.Contains(sl.Group.Id.ToString())) && s.End_Date > DateTime.Now).Select(s => s.Id.ToString()).ToList();


                //        validSurveys = validSurveys.Except(surveyId).ToList();

                //        if (validSurveys.Count > 0)
                //        {
                //            emailsList.Add(userInDb.Email);
                //            break;
                //        }

                //    }
                //}


                //return emailsList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private List<string> GetEmailsSurveys(List<string> groupId, List<string> surveyId)
        {
            try
            {

                List<string> emailsList = new List<string>();


                foreach (var group in groupId)
                {
                    var allUsers = dbContext
                        .Users
                        .Include(u => u.GroupsLink)
                        .Where(u => u.GroupsLink.Any(gl => gl.Group.Id.ToString() == group));

                    foreach (var user in allUsers)
                    {
                        var allGroups = dbContext
                            .Groups
                            .Include(g => g.GroupsLink)
                            .Include(g => g.SurveysList)
                            .Where(g => g.GroupsLink.Any(gl => gl.UserId == user.Id));

                        var allValidSurveys = dbContext
                            .Surveys
                            .Include(s => s.SurveysList)
                            .Where(s => s.SurveysList.Any(sl => allGroups.Any(ag => ag.Id == sl.GroupId) && s.End_Date > DateTime.Now)).ToList();

                        foreach (var survey in surveyId)
                        {
                            if (!allValidSurveys.Select(s => s.Id.ToString()).Contains(survey))
                            {
                                emailsList.Add(user.Email);
                                break;
                            }
                        }
                    }
                }

                return emailsList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
