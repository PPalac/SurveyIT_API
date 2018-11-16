using SurveyIT.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Helpers
{
    public class CommonResult
    {
        public CommonResultState StateMessage { get; set; }

        public string Message { get; set; }

        public CommonResult()
        {

        }

        public CommonResult(CommonResultState stateMessage, string text)
        {
            StateMessage = stateMessage;
            Message = text;
        }
    }
}
