

using System.ComponentModel.DataAnnotations;

namespace BucketProject.UI.ViewModels.ViewModels
{
    public class UserViewModel
    {
        public byte[] Picture { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters.")]
        public string Username { get; set; }

        public string Email { get; set; }

        public DateOnly CreatedAt { get; set; }

        public string Nationality { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }


    }
}
