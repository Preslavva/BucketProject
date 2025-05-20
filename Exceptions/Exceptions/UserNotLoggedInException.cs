using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions.Exceptions
{
    public class UserNotLoggedInException:Exception
    {
        public UserNotLoggedInException() : base("User not logged in.") { }
    }
}
