namespace BucketProject.BLL.Business_Logic.DTOs
{
    public class User
    {
       public int Id { get; set; }
        public string Username { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Nationality { get; set; }

        public string Gender { get; set; }
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Salt { get; set; }

        public byte[] Picture { get; set; }
        public string Role { get; set; }

    }

    }


