using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    public class InvalidLoginException:Exception
    {
        public InvalidLoginException() : base("Wrong username or password")
        {
        }
    }
}
