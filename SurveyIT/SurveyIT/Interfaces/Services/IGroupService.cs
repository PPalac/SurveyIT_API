using SurveyIT.Enums;
using SurveyIT.Helpers;
using SurveyIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Interfaces.Services
{
    public interface IGroupService
    {
        Task<CommonResult> AddGroup(GroupModel group);

        CommonResult ValidationGroup(GroupModel group);

        Task<CommonResult> DeleteGroup(string groupName);

        Task<CommonResult> EditGroup(GroupModel group, string newGroupName);

        List<GroupModel> GetAllGroups();

        GroupModel GetOneGroup(string groupId);

        List<GroupModel> GetGroupsForSurvey(int surveyId);
    }
}
