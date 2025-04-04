using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BucketProject.UI.ViewModels.ViewModels
{
    public class UserViewModel
    {
        public byte[] Picture { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public DateOnly CreatedAt { get; set; }

        public string Nationality { get; set; }
        public string Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }


    }
}
