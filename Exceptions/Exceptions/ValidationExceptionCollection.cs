using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions.Exceptions
{
    public class ValidationExceptionCollection:Exception
    {
        public List<string> Errors { get; }

        public ValidationExceptionCollection(List<string> errors)
            : base("Validation failed with one or more errors.")
        {
            Errors = errors;
        }

       
    }
}
