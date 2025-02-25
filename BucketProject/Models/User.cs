namespace BucketProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string Picture { get; set; }

        //database reading 
        public User(int id, string username, string email, string password, string picture)
        {
            this.Id = id;
            this.Username = username;
            this.Email = email;
            this.Password = password;
            this.Picture = picture;
        }
        public User(string username, string email, string password)
        {
            this.Username = username;
            this.Email = email;
            this.Password = password;
        }

        public User() { }
    }
}
