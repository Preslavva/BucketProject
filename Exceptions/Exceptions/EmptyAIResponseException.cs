using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions.Exceptions
{
    public class EmptyAIResponseException:Exception
    {
        public EmptyAIResponseException() : base("Failed to generate sub goal.")
        {

        }
    }
}
