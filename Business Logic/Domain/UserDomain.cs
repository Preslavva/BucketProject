using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BucketProject.BLLBusiness_Logic.Domain
{
    public class UserDomain
    {
       
            public string Username { get; set; }

            public string Email { get; set; }

            public DateOnly DateOfBirth { get; set; }

            public string Nationality { get; set; }

            public string Gender { get; set; }

            public DateOnly CreatedAt { get; set; } 
            public string Password { get; set; }
           
        }

    }


