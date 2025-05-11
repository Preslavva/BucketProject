

using System.ComponentModel.DataAnnotations;

namespace BucketProject.UI.ViewModels.ViewModels
{
    public class UserViewModel
    {
        public byte[] Picture { get; set; }

        [Required(ErrorMessage = "The new Username cannot be empty or whitespace.")]
        public string Username { get; set; }

        public string Email { get; set; }

        public DateOnly CreatedAt { get; set; }

        public string Nationality { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }


    }
}
