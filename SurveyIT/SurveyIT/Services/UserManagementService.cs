using SurveyIT.DB;
using SurveyIT.Helpers;
using SurveyIT.Interfaces.Services;
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

        public UserManagementService(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<CommonResult> AssignUsersToGroup(List<string> groupId, List<string> userId)
        {
            try
            {
                if(groupId!=null && userId!=null)
                {
                    foreach (var groups in groupId)
                    {
                        var group = dbContext.Groups.FirstOrDefault(x => x.Id.ToString() == groups);

                        if(group!=null)
                        {
                            foreach (var users in userId)
                            {
                                var user = dbContext.Users.FirstOrDefault(u => u.Id == users && u.Role==Enums.Role.User);

                                if(user!=null)
                                {
                                    var groupLink = dbContext.GroupsLink.Where(x => x.User.Id == user.Id && x.Group.Id == group.Id);

                                    if (groupLink == null)
                                    {
                                        dbContext.GroupsLink.Add(new Models.DBModels.GroupsLink { User = user, Group = group });
                                    }
                                }
                                else
                                {
                                    return new CommonResult(Enums.CommonResultState.Warning, "Nie wszystkie obiekty istnieja");
                                }
                            }
                            await dbContext.SaveChangesAsync();
                            return new CommonResult(Enums.CommonResultState.OK, "Przypisanie przebieglo pomyslnie");
                        }
                        else
                        {
                            return new CommonResult(Enums.CommonResultState.Warning, "Nie wszystkie obiekty istnieja");
                        }
                    }                 
                }

                return new CommonResult(Enums.CommonResultState.Warning, "Podane obiekty nie istnieja");
            }
            catch (Exception ex)
            {
                return new CommonResult(Enums.CommonResultState.Error, "Blad podczas przypisywania");
            }
        }

        public List<HelperIdModel> DisplayAllGroup()
        {
            try
            {
                List<HelperIdModel> groupList = new List<HelperIdModel>();
                var groups = dbContext.Groups.ToList();

                if (groups != null)
                {
                    foreach (var group in groups)
                    {
                        groupList.Add(new HelperIdModel { FirstId=group.Id.ToString(), SecondId = group.Name });
                    }

                    return groupList;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd wyswietlania");
            }
        }

        public List<HelperIdModel> DisplayAllUser()
        {
            try
            {
                List<HelperIdModel> userList = new List<HelperIdModel>();
                var users = dbContext.Users.ToList().Where(x=>x.Role== Enums.Role.User);

                if (users != null)
                {
                    foreach (var user in users)
                    {
                        userList.Add(new HelperIdModel { FirstId = user.Id.ToString(), SecondId = user.UserName });
                    }

                    return userList;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd wyswietlania");
            }
        }

        public List<HelperIdModel> DisplayAssignedUsers(string groupId)
        {
            try
            {
                List<HelperIdModel> userList = new List<HelperIdModel>();
                var groupLinks = dbContext.GroupsLink.Where(g => g.Group.Id.ToString() == groupId);

                if (groupLinks != null)
                {
                    foreach (var groupLink in groupLinks)
                    {
                        if(groupLink.User.Role == Enums.Role.User)
                            userList.Add(new HelperIdModel { FirstId = groupLink.User.Id, SecondId = groupLink.User.UserName });
                    }

                    return userList;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd wyswietlania");
            }
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
                                var user = dbContext.Users.FirstOrDefault(u => u.Id == users && u.Role==Enums.Role.User);

                                if (user != null)
                                {
                                    var groupLink = dbContext.GroupsLink.Where(x => x.User.Id == user.Id && x.Group.Id == group.Id);

                                    if (groupLink != null)
                                    {
                                        dbContext.GroupsLink.Remove(new Models.DBModels.GroupsLink { User = user, Group = group });
                                    }
                                }
                                else
                                {
                                    return new CommonResult(Enums.CommonResultState.Warning, "Nie wszystkie obiekty istnieja");
                                }
                            }

                            await dbContext.SaveChangesAsync();
                            return new CommonResult(Enums.CommonResultState.OK, "Odprzypisanie przebieglo pomyslnie");
                        }
                        else
                        {
                            return new CommonResult(Enums.CommonResultState.Warning, "Nie wszystkie obiekty istnieja");
                        }
                    }
                }

                return new CommonResult(Enums.CommonResultState.Warning, "Podane obiekty nie istnieja");
            }
            catch (Exception ex)
            {
                return new CommonResult(Enums.CommonResultState.Error, "Blad podczas odprzypisywania");
            }
        }
    }
}
