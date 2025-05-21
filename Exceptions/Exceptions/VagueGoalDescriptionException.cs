using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions.Exceptions
{
    public class VagueGoalDescriptionException:Exception
    {
        public VagueGoalDescriptionException()
        : base("The goal description is too vague or too short to generate meaningful sub-goals.") { }
    }
}
