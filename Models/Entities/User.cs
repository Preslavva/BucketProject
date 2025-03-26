namespace BucketProject.DAL.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public byte[] Picture { get; set; }

    public string Salt { get; set; }

    //database reading 
    public User(int id, string username, string email, string password, byte[] picture, string salt)
    {
        this.Id = id;
        this.Username = username;
        this.Email = email;
        this.Password = password;
        this.Picture = picture;
        this.Salt = salt;
    }
    public User(string username, string email, string password, string salt)
    {
        this.Username = username;
        this.Email = email;
        this.Password = password;
        this.Salt = salt;
    }

    public User() { }
}
