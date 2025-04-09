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

    private DateOnly _dateOfBirth;

    public DateOnly DateOfBirth
    {
        get => _dateOfBirth;
        private set => _dateOfBirth = value;
    }

    //public int Age
    //{
    //    get
    //    {
    //        var today = DateOnly.FromDateTime(DateTime.Today);
    //        var age = today.Year - _dateOfBirth.Year;
    //        if (_dateOfBirth > today.AddYears(-age))
    //        {
    //            age--;
    //        }
    //        return age;
    //    }
    //    set
    //    {
    //        _dateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-value));
    //    }
    //}


    public string Gender { get; private set; }

    public DateOnly CreatedAt { get; private set; } = DateOnly.FromDateTime(DateTime.Today);


    // Constructor for database reading
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

    public User(string username, string email, string password, string salt, string nationality, DateOnly dateOfBirth, string gender)
    {
        this.Username = username;
        this.Email = email;
        this.Password = password;
        this.Salt = salt;
        this.Nationality = nationality;
        this.DateOfBirth = dateOfBirth;
        this.Gender = gender;
        this.CreatedAt = DateOnly.FromDateTime(DateTime.Today);
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
