using SurveyIT.DB;
using SurveyIT.Helpers;
using SurveyIT.Interfaces.Services;
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

        public CommonResult AssignUsersToGroup(List<string> groupId, List<string> userId)
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
                                var user = dbContext.Users.FirstOrDefault(u => u.Id == users);

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
                            dbContext.SaveChanges();
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

        public SortedList<string, string> DisplayAllGroup()
        {
            try
            {
                SortedList<string, string> groupList = new SortedList<string, string>();
                var groups = dbContext.Groups.ToList();

                if (groups != null)
                {
                    foreach (var group in groups)
                    {
                        groupList.Add(group.Id.ToString(), group.Name);
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

        public SortedList<string, string> DisplayAllUser()
        {
            try
            {
                SortedList<string, string> userList = new SortedList<string, string>();
                var users = dbContext.Users.ToList();

                if (users != null)
                {
                    foreach (var user in users)
                    {
                        userList.Add(user.Id.ToString(), user.UserName);
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

        public SortedList<string, string> DisplayAssignedUsers(string groupId)
        {
            try
            {
                SortedList<string, string> userList = new SortedList<string, string>();
                var groupLinks = dbContext.GroupsLink.Where(g => g.Group.Id.ToString() == groupId);

                if (groupLinks != null)
                {
                    foreach (var groupLink in groupLinks)
                    {
                        userList.Add(groupLink.User.Id,groupLink.User.UserName);
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

        public CommonResult UnAssignUsersInGroup(List<string> groupId, List<string> userId)
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
                                var user = dbContext.Users.FirstOrDefault(u => u.Id == users);

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

                            dbContext.SaveChanges();
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
