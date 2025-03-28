using System.Diagnostics;
using BucketProject.DAL.Models.Enums;

namespace BucketProject.DAL.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public byte[] Picture { get; set; }

    public string Salt { get; set; }

    public string Nationality { get; set; }

    private DateTime _dateOfBirth;

    public DateTime DateOfBirth
    {
        get => _dateOfBirth;
        set => _dateOfBirth = value;
    }

    public int Age
    {
        get
        {
            var today = DateTime.Today;
            var age = today.Year - _dateOfBirth.Year;
            if (_dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }
        set
        {
            _dateOfBirth = DateTime.Today.AddYears(-value);
        }
    }

    public string Gender { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Constructor for database reading
    public User(int id, string username, string email, string password, byte[] picture, string salt, string nationality, DateTime dateOfBirth, string gender, DateTime createdAt)
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

    public User(string username, string email, string password, string salt, string nationality, DateTime dateOfBirth, string gender)
    {
        this.Username = username;
        this.Email = email;
        this.Password = password;
        this.Salt = salt;
        this.Nationality = nationality;
        this.DateOfBirth = dateOfBirth;
        this.Gender = gender;
        this.CreatedAt = DateTime.Now; ;
    }

    public User() { }
}
