using SurveyIT.Helpers;
using SurveyIT.Models.HelperModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Interfaces.Services
{
    public interface IUserManagementService
    {
        List<HelperIdModel> DisplayAllUser();

        List<HelperIdModel> DisplayAllGroup();

        List<HelperIdModel> DisplayAssignedUsers(string groupId);

        Task<CommonResult> AssignUsersToGroup(List<string> groupId, List<string> userId);

        Task<CommonResult> UnAssignUsersInGroup(List<string> groupId, List<string> userId);
    }
}
