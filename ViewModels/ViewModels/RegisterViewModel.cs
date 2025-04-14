using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace BucketProject.UI.ViewModels.ViewModels
{
    public class RegisterViewModel
    {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateOnly DateOfBirth { get; set; }
            
            [Required]
            [Display(Name = "Nationality")]
            public string Nationality { get; set; }
            
            [Required]
            [Display(Name = "Gender")]
            public string Gender { get; set; }

            [Required]
            [StringLength(50, MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
        }

    }

