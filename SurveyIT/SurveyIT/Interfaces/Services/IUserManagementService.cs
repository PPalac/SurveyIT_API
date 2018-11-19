using SurveyIT.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Interfaces.Services
{
    public interface IUserManagementService
    {
        SortedList<string,string> DisplayAllUser();

        SortedList<string, string> DisplayAllGroup();

        SortedList<string, string> DisplayAssignedUsers(string groupId);

        CommonResult AssignUsersToGroup(List<string> groupId, List<string> userId);

        CommonResult UnAssignUsersInGroup(List<string> groupId, List<string> userId);
    }
}
