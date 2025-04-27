using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BucketProject.UI.ViewModels.ViewModels
{
    public class RegisterViewModel
    {
        
            [Required]
            public string Username { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }

            [Required, DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }      // <-- swapped to DateTime

            [Required]
            public string Nationality { get; set; }

            [Required]
            public string Gender { get; set; }

            [Required, StringLength(50, MinimumLength = 6), DataType(DataType.Password)]
            public string Password { get; set; }

            [Required, Compare(nameof(Password))]
            public string ConfirmPassword { get; set; }

            [ValidateNever]                                // <-- not validated
            public IEnumerable<SelectListItem> Countries { get; set; }
        }

    
}

