using Microsoft.EntityFrameworkCore;
using SurveyIT.DB;
using SurveyIT.Helpers;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;
using SurveyIT.Models.HelperModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Services
{
    public class UserManagementService : IUserManagementService
    {
        private MyDbContext dbContext;
        private EmailService emailService;

        public UserManagementService(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
            emailService = new EmailService(dbContext);
        }

        public async Task<CommonResult> AssignUsersToGroup(List<string> userId, List<string> groupId)
        {
            try
            {
                if (groupId != null && userId != null)
                {
                    foreach (var groups in groupId)
                    {
                        var group = dbContext.Groups.FirstOrDefault(x => x.Id.ToString() == groups);

                        if (group != null)
                        {
                            List<string> emails = new List<string>();

                            foreach (var users in userId)
                            {
                                var user = dbContext.Users.FirstOrDefault(u => u.Id == users && u.Role == Enums.Role.User);

                                if (user != null)
                                {
                                    var groupLink = dbContext.GroupsLink.Where(x => x.User.Id == user.Id && x.Group.Id == group.Id).ToList();
                                    emails.Add(user.Email);

                                    if (groupLink.Count == 0)
                                    {
                                        dbContext.GroupsLink.Add(new Models.DBModels.GroupsLink { User = user, Group = group });
                                    }
                                    //else
                                    //    return new CommonResult(Enums.CommonResultState.Warning, "Takie przypisanie juz istnieje");
                                }
                                else
                                {
                                    return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.NoDataToAssign);
                                }
                            }

                            
                        }
                        else
                        {
                            return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.NoDataToAssign);
                        }
                    }

                    emailService.SendEmailsGroups(userId, groupId);
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

        public List<HelperIdGroupModel> DisplayAllGroup()
        {
            try
            {
                List<HelperIdGroupModel> groupList = new List<HelperIdGroupModel>();
                var groups = dbContext.Groups.ToList();

                if (groups != null)
                {
                    foreach (var group in groups)
                    {
                        groupList.Add(new HelperIdGroupModel { Id = group.Id.ToString(), Name = group.Name });
                    }

                    return groupList;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(Properties.Resources.ErrorDisplay);
            }
        }

        public List<HelperIdUserModel> DisplayAllUser()
        {
            try
            {
                List<HelperIdUserModel> userList = new List<HelperIdUserModel>();
                var users = dbContext.Users.ToList().Where(x => x.Role == Enums.Role.User);

                if (users != null)
                {
                    foreach (var user in users)
                    {
                        userList.Add(new HelperIdUserModel { Id = user.Id.ToString(), Name = user.UserName });
                    }

                    return userList;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(Properties.Resources.ErrorDisplay);
            }
        }

        public List<HelperIdGroupModel> DisplayAssignedUsers(string groupId)
        {
            try
            {
                var newGroupId = int.Parse(groupId);
                List<HelperIdGroupModel> userList = new List<HelperIdGroupModel>();

                var users = dbContext
                    .GroupsLink
                    .Where(gl => gl.Group.Id == newGroupId)
                    .Select(gl => gl.User.Id).ToList();

                if (users != null)
                {
                    foreach (var user in users)
                    {
                        var userDB = dbContext.Users.FirstOrDefault(u => u.Id == user);

                        if (userDB != null)
                        {
                            if (userDB.Role == Enums.Role.User)
                                userList.Add(new HelperIdGroupModel { Id = userDB.Id, Name = userDB.UserName });
                        }


                    }

                    return userList;
                }


                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(Properties.Resources.ErrorDisplay);
            }
        }

        public List<UserModel> GetUnusignedUsers(string groupId)
        {
            if (int.TryParse(groupId, out int gId))
                return dbContext.Users
                    .Where(u => u.Role == Enums.Role.User && !u.GroupsLink.Any(gl => gl.Group.Id == gId))
                    .Select(u => new UserModel { Id = u.Id, Username = u.UserName })
                    .ToList();

            return new List<UserModel>();
        }

        public async Task<CommonResult> UnAssignUsersInGroup(List<string> groupId, List<string> userId)
        {
            try
            {
                if (groupId != null && userId != null)
                {
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
                                    var groupLink = dbContext.GroupsLink.FirstOrDefault(x => x.User.Id == user.Id && x.Group.Id == group.Id);

                                    if (groupLink != null)
                                    {
                                        dbContext.Users.FirstOrDefault(u => u.Id == users).GroupsLink.Remove(groupLink);
                                        dbContext.Groups.FirstOrDefault(g=>g.Id == int.Parse(groups)).GroupsLink.Remove(groupLink);
                                        dbContext.GroupsLink.Remove(groupLink);
                                    }
                                }
                                else
                                {
                                    return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.NoDataExist);
                                }
                            }

                            
                        }
                        else
                        {
                            return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.NoDataExist);
                        }
                    }

                    await dbContext.SaveChangesAsync();
                    return new CommonResult(Enums.CommonResultState.OK, Properties.Resources.UnAssignCorrect);
                }

                return new CommonResult(Enums.CommonResultState.Warning, Properties.Resources.NoDataExist);
            }
            catch (Exception ex)
            {
                return new CommonResult(Enums.CommonResultState.Error, Properties.Resources.Error);
            }
        }
    }
}
