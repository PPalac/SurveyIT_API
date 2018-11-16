using Microsoft.EntityFrameworkCore;
using SurveyIT.DB;
using SurveyIT.Enums;
using SurveyIT.Helpers;
using SurveyIT.Interfaces.Services;
using SurveyIT.Models;
using SurveyIT.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SurveyIT.Services
{
    public class GroupService : IGroupService
    {
        private MyDbContext dbContext;

        public GroupService(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public CommonResult ValidationGroup(GroupModel group)
        {
            if (group != null && !string.IsNullOrEmpty(group.Name))
            {
                if (Regex.IsMatch(group.Name.First().ToString(), "[A-Z]"))
                {
                    if (group.Name.Length > 3)
                    {
                        if (!dbContext.Groups.Any(g => g.Name == group.Name))
                        {
                            return new CommonResult(CommonResultState.OK, "Walidacja poprawna");
                        }

                        return new CommonResult(CommonResultState.Warning, "Grupa o takiej nazwie istnieje");
                    }

                    return new CommonResult(CommonResultState.Warning, "Za krótka nazwa");
                }

                return new CommonResult(CommonResultState.Warning, "Nazwa powinna zaczynać się od wielkiej litery");
            }

            return new CommonResult(CommonResultState.Error, "Błąd walidacji");
        }

        public async Task<CommonResult> AddGroup(GroupModel group)
        {
            try
            {
                CommonResult validationResult = ValidationGroup(group);

                if (validationResult.StateMessage == CommonResultState.OK)
                {
                    var newGroup = new Groups();
                    newGroup.Name = group.Name;
                    newGroup.GroupsLink = new List<GroupsLink>();

                    var users = group.UserId;

                    if (users != null)
                    {
                        foreach (var user in users)
                        {
                            newGroup.GroupsLink.Add(new GroupsLink { UserId = user });
                        }
                    }

                    dbContext.Groups.Add(newGroup);

                    await dbContext.SaveChangesAsync();

                    return validationResult; //todo: zwracanie nowego resultu z wiadomoscia ze grupa dodana
                }

                return validationResult;
            }
            catch (Exception ex)
            {
                return new CommonResult(CommonResultState.Error, "Błąd");
            }

        }

        public async Task<CommonResult> DeleteGroup(string groupName)
        {
            try
            {
                if (groupName != null) //todo: isnullorempty
                {
                    var deleteGroup = dbContext.Groups.FirstOrDefault(g => g.Name == groupName);
                    var deleteGroupsLink = dbContext.GroupsLink.Where(gl => gl.Group.Id == deleteGroup.Id);

                    foreach (var group in deleteGroupsLink)
                    {
                        dbContext.GroupsLink.Remove(group);
                    }

                    dbContext.Groups.Remove(deleteGroup);

                    await dbContext.SaveChangesAsync();

                    return new CommonResult(CommonResultState.OK, "Usunieto grupe");
                }

                return new CommonResult(CommonResultState.Error, "Grupa nie istnieje");
            }
            catch (Exception)
            {
                return new CommonResult(CommonResultState.Error, "Błąd podczas usuwania grupy");
            }

        }

        public async Task<CommonResult> EditGroup(GroupModel group, string newGroupName)
        {
            try
            {
                GroupModel groupModel = new GroupModel
                {
                    Name = newGroupName
                };

                var result = ValidationGroup(groupModel);

                if (result.StateMessage == CommonResultState.OK)
                {
                    dbContext.Groups.FirstOrDefault(g => g.Name == group.Name).Name = newGroupName;

                    await dbContext.SaveChangesAsync();

                    return new CommonResult(CommonResultState.OK, "Zmiana nazwy powiodła sie");
                }

                return result;
            }
            catch (Exception)
            {
                return new CommonResult(CommonResultState.Error, "Błąd podczas zmiany nazwy grupy");
            }
        }

        public List<string> GetAllGroups() //todo: zwrocic groupmodel z uzytkownikami
        {
            try
            {
                List<string> groupList = new List<string>();
                var groups = dbContext.Groups.ToList();

                if(groups!=null)
                {
                    foreach (var group in groups)
                    {
                        groupList.Add(group.Name);
                    }

                    return groupList;
                }

                return null;
            }
            catch (Exception)
            {
                throw new Exception("")
            }
        }
    }
}
