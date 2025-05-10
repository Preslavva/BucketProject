

namespace BucketProject.DAL.Models.Entities;

public class UserEntity
{
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }

    public byte[] Picture { get; private set; }

    public string Salt { get; private set; }

    public string Nationality { get; private set; }
    public DateTime DateOfBirth { get; private set; }

    public string Gender { get; private set; }

    public DateOnly CreatedAt { get; private set; } = DateOnly.FromDateTime(DateTime.Today);
    public string Role { get; private set; }


    public UserEntity(int id, string username, string email, string password, byte[] picture, string salt, string nationality, DateTime dateOfBirth, string gender, DateOnly createdAt, string role )
    {
        this.Id = id;
        this.Username = username;
        this.Email = email;
        this.Password = password;
        this.Picture = picture;
        this.Salt = salt;
        this.Nationality = nationality;
        this.DateOfBirth = dateOfBirth; 
        this.Gender = gender;
        this.CreatedAt = createdAt;
        this.Role = role;
    }

  

    public UserEntity(int id, string username, byte[] picture) {
        this.Id = id;
        this.Username = username;
        this.Picture = picture;
    }
    public UserEntity(int id, string username)
    {
        this.Id = id;
        this.Username = username;
      
    }
    public UserEntity() { }
  

    public void SetPasswordAndSalt(string password, string salt)
    {
        Password = password;
        Salt = salt;
    }
}
