using SurveyIT.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyIT.Helpers
{
    public class ResourceMessages
    {
        public EnumStateMessage StateMessage { get; set; }

        public string Text { get; set; }

        public ResourceMessages()
        {

        }

        public ResourceMessages(EnumStateMessage stateMessage, string text)
        {
            StateMessage = stateMessage;
            Text = text;
        }
    }
}
