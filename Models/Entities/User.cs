using System.Diagnostics;
using BucketProject.DAL.Models.Enums;

namespace BucketProject.DAL.Models.Entities;

public class User
{
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }

    public byte[] Picture { get; private set; }

    public string Salt { get; private set; }

    public string Nationality { get; private set; }
    public DateOnly DateOfBirth { get; private set; }

    public string Gender { get; private set; }

    public DateOnly CreatedAt { get; private set; } = DateOnly.FromDateTime(DateTime.Today);


    public User(int id, string username, string email, string password, byte[] picture, string salt, string nationality, DateOnly dateOfBirth, string gender, DateOnly createdAt)
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
    }

  

    public User() { }

    public void UpdatePicture(byte[] newPicture)
    {
        Picture = newPicture;
    }

    public void ChangeEmail(string newEmail)
    {
        Email = newEmail;
    }

    public void ChangePassword(string newPassword, string newSalt)
    {
        Password = newPassword;
        Salt = newSalt;
    }
}
