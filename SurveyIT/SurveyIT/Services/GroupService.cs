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
                            return new CommonResult(CommonResultState.OK, Properties.Resources.CorrectValidation);
                        }

                        return new CommonResult(CommonResultState.Warning, Properties.Resources.GroupNameUsed);
                    }

                    return new CommonResult(CommonResultState.Warning, Properties.Resources.ShortGroupName);
                }

                return new CommonResult(CommonResultState.Warning, Properties.Resources.NotValidName);
            }

            return new CommonResult(CommonResultState.Error, Properties.Resources.ErrorValidation);
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
                    validationResult.Message = Properties.Resources.GroupAdded;
                    return validationResult;
                }

                return validationResult;
            }
            catch (Exception ex)
            {
                return new CommonResult(CommonResultState.Error, Properties.Resources.Error);
            }

        }

        public async Task<CommonResult> DeleteGroup(string groupName)
        {
            try
            {
                if (!string.IsNullOrEmpty(groupName)) 
                {
                    var deleteGroup = dbContext.Groups.FirstOrDefault(g => g.Name == groupName);
                    var deleteGroupsLink = dbContext.GroupsLink.Where(gl => gl.Group.Id == deleteGroup.Id);

                    if (deleteGroupsLink != null)
                    {
                        foreach (var group in deleteGroupsLink)
                        {
                            dbContext.GroupsLink.Remove(group);
                        }
                    }

                    dbContext.Groups.Remove(deleteGroup);

                    await dbContext.SaveChangesAsync();

                    return new CommonResult(CommonResultState.OK, Properties.Resources.GroupDeleted);
                }

                return new CommonResult(CommonResultState.Error, Properties.Resources.GroupNotExist);
            }
            catch (Exception ex)
            {
                return new CommonResult(CommonResultState.Error, Properties.Resources.Error);
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

                    return new CommonResult(CommonResultState.OK, Properties.Resources.ChangedName);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new CommonResult(CommonResultState.Error, Properties.Resources.Error);
            }
        }

        public List<GroupModel> GetAllGroups()
        {
            try
            {
                List<GroupModel> groupList = new List<GroupModel>();
                var groups = dbContext.Groups.Include(g => g.GroupsLink).ToList();

                if(groups!=null)
                {
                    foreach (var group in groups)
                    {
                        GroupModel newGroupModel = new GroupModel();
                        newGroupModel.Name = group.Name;
                        newGroupModel.Id = group.Id;

                        foreach (var groupLink in group.GroupsLink)
                        {
                            newGroupModel.UserId.Add(groupLink.UserId);
                        }
                        groupList.Add(newGroupModel);
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

        public GroupModel GetOneGroup(string groupId)
        {
            try
            {
                if(!string.IsNullOrEmpty(groupId))
                {
                    var group = dbContext.Groups.FirstOrDefault(g => g.Id.ToString() == groupId);

                    if(group!=null)
                    {
                        GroupModel newGroupModel = new GroupModel();
                        newGroupModel.Id = group.Id;
                        newGroupModel.Name = group.Name;

                        List<string> userId = new List<string>();

                        var groupLink = dbContext.GroupsLink.Where(x => x.Group.Id == group.Id);

                        foreach (var user in groupLink)
                        {
                            userId.Add(user.UserId);
                        }

                        newGroupModel.UserId = userId;

                        return newGroupModel;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(Properties.Resources.Error);
            }
        }

        public List<GroupModel> GetGroupsForSurvey(int surveyId)
        {
            var result = dbContext.Groups.Where(g => g.SurveysList.Select(sl => sl.Survey.Id).Any(sl => sl == surveyId)).Select(g => new GroupModel { Id = g.Id, Name = g.Name }).ToList(); ;

            return result;
        }
    }
}
