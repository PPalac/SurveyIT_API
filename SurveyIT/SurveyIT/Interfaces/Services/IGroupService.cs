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
        Task<ResourceMessages> AddGroup(GroupModel group);

        ResourceMessages ValidationGroup(GroupModel group);

        Task<ResourceMessages> DeleteGroup(string groupName);

        Task<ResourceMessages> EditGroup(GroupModel group, string newGroupName);

        List<string> GetAllGroups();
    }
}
