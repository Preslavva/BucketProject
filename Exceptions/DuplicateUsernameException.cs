using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions.Exceptions
{
    public class DuplicateUsernameException : Exception
    {
        public DuplicateUsernameException() : base("Username is already taken.") { }
    }
}
