using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions.Exceptions
{
    public class AIRequestFailedException:Exception
    {
        public string UserMessage { get; }

        public AIRequestFailedException(string userMessage, string developerMessage)
            : base(developerMessage)
        {
            UserMessage = userMessage;
        }
    }
}
