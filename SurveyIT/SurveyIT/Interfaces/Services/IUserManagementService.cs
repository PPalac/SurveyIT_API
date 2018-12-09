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
        List<HelperIdUserModel> DisplayAllUser();

        List<HelperIdGroupModel> DisplayAllGroup();

        List<HelperIdGroupModel> DisplayAssignedUsers(string groupId);

        Task<CommonResult> AssignUsersToGroup(List<string> userId, List<string> groupId);

        Task<CommonResult> UnAssignUsersInGroup(List<string> groupId, List<string> userId);
    }
}
