namespace BucketProject.BLLBusiness_Logic.Domain
{
    public class User
    {
       
        public string Username { get; set; }

        public string Email { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Nationality { get; set; }

        public string Gender { get; set; }
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public string Password { get; set; }

        public byte[] Picture { get; set; }

    }

    }


